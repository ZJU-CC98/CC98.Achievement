using CC98.Achievement.AppManagement;
using CC98.Achievement.Data;
using CC98.Achievement.Settings;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CC98.Achievement.Services;


/// <summary>
/// 提供定期更新用户参与人数的服务。
/// </summary>
public class UpdateUserCountService : IHostedService, IDisposable, IAsyncDisposable
{
	/// <summary>
	/// 提供服务领域功能。
	/// </summary>
	private IServiceScopeFactory ServiceScopeFactory { get; }

	/// <summary>
	/// 内部成就服务。
	/// </summary>
	private AchievementBackService InnerService { get; }

	/// <summary>
	/// 数据更新设置。
	/// </summary>
	private DataUpdatePeriodSetting DataUpdatePeriodSetting { get; }

	/// <summary>
	/// 定时器对象。
	/// </summary>
	private Timer Timer { get; }

	/// <summary>
	/// 初始化 <see cref="UpdateUserCountService"/> 服务的新实例。
	/// </summary>
	/// <param name="serviceScopeFactory"><see cref="IServiceScopeFactory"/> 服务对象。</param>
	/// <param name="innerService"><see cref="AchievementBackService"/> 服务对象。</param>
	/// <param name="dataUpdatePeriodSettingOptions"><see cref="IOptions{DataUpdatePeriodSetting}"/> 服务对象。</param>
	public UpdateUserCountService(IServiceScopeFactory serviceScopeFactory, AchievementBackService innerService,
		IOptions<DataUpdatePeriodSetting> dataUpdatePeriodSettingOptions)
	{
		ServiceScopeFactory = serviceScopeFactory;
		InnerService = innerService;
		DataUpdatePeriodSetting = dataUpdatePeriodSettingOptions.Value;
		Timer = new(TimerCallback);
	}

	private async void TimerCallback(object? state)
	{
		var cancellationToken = CancellationToken.None;

		await using var scope = ServiceScopeFactory.CreateAsyncScope();
		await using var dbContext = scope.ServiceProvider.GetRequiredService<AchievementDbContext>();

		var userCount =
			await (from i in dbContext.Records
				   select i.UserName).Distinct().CountAsync(cancellationToken);

		await InnerService.UpdateUserCountAsync(userCount, cancellationToken);
	}

	/// <inheritdoc />
	public Task StartAsync(CancellationToken cancellationToken)
	{
		Timer.Change(TimeSpan.Zero, DataUpdatePeriodSetting.UserCount ?? Timeout.InfiniteTimeSpan);
		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public Task StopAsync(CancellationToken cancellationToken)
	{
		Timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		Timer.Dispose();
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		await Timer.DisposeAsync();
		GC.SuppressFinalize(this);
	}
}
