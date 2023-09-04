using System.Data;

using CC98.Achievement.AppManagement;

namespace CC98.Achievement.Services;

/// <summary>
/// 提供 <see cref="AchievementService"/> 的 ASP.NET Core 服务实现。
/// </summary>
public class AchievementBackService : IDisposable
{
	/// <summary>
	/// 日志服务对象。
	/// </summary>
	private ILogger<AchievementBackService> Logger { get; }

	/// <summary>
	/// 初始化 <see cref="AchievementBackService"/> 对象的新实例。
	/// </summary>
	/// <param name="configuration"><see cref="IConfiguration"/> 服务对象。</param>
	/// <param name="logger"><see cref="ILogger{AchievementBackService}"/> 服务对象。</param>
	public AchievementBackService(IConfiguration configuration, ILogger<AchievementBackService> logger)
	{
		Logger = logger;
		// 配置内部服务对象。
		InnerService = CreateInnerService(HttpClient, configuration);
	}

	/// <summary>
	/// 创建可用后续使用的 <see cref="AchievementService"/> 对象。
	/// </summary>
	/// <returns>可供后续使用的 <see cref="AchievementService"/> 对象。</returns>
	/// <exception cref="InvalidOperationException">一个或多个系统参数无效。</exception>
	private static AchievementService CreateInnerService(HttpClient httpClient, IConfiguration configuration)
	{
		// 配置成就服务相关设置
		var options = new AchievementServiceOptions
		{
			ClientId = configuration["Authentication:CC98:ClientId"] ?? throw new InvalidOperationException("未配置成就系统 ClientID。"),
			ClientSecret = configuration["Authentication:CC98:ClientSecret"] ?? throw new InvalidOperationException("未配置成就系统 ClientSecret。"),
			Authority = configuration["Authentication:CC98:Authority"] ?? throw new InvalidOperationException("未配置成就系统授权机构 URL。"),
			ApiBaseUri = configuration["Api:BaseUri"] ?? throw new InvalidOperationException("未配置成就系统 API 地址。"),
		};

		// 设置内部服务
		return new(httpClient, options);
	}

	/// <summary>
	/// 更新用户使用人数。
	/// </summary>
	/// <param name="userCount">新用户使用人数。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	public async Task UpdateUserCountAsync(int userCount, CancellationToken cancellationToken = default)
	{
		Logger.LogDebug("尝试更新使用人数，新人数 = {UserCount}", userCount);

		try
		{
			var categoryInfo = await InnerService.GetCategoryInfoAsync(cancellationToken);
			categoryInfo.UserCount = userCount;
			await InnerService.UpdateCategoryInfoAsync(categoryInfo, cancellationToken);

			Logger.LogInformation("更新用户人数成功");
		}
		catch (Exception ex)
		{
			Logger.LogError("无法更新成就系统使用人数，错误 = {Error}", ex.GetBaseMessage());
		}
	}


	/// <summary>
	/// HTTP 请求服务对象。
	/// </summary>
	private HttpClient HttpClient { get; } = new();

	/// <summary>
	/// 内部服务对象。
	/// </summary>
	public AchievementService InnerService { get; private set; }

	/// <inheritdoc />
	public void Dispose()
	{
		HttpClient.Dispose();
		GC.SuppressFinalize(this);
	}
}