using CC98.Achievement.AppManagement;

namespace CC98.Achievement.Services;

/// <summary>
/// 提供 <see cref="AchievementService"/> 的 ASP.NET Core 服务实现。
/// </summary>
public class AchievementBackService : IDisposable
{
	public AchievementBackService(IConfiguration configuration)
	{
		Configuration = configuration;
		InitializeInnerService();
	}

	/// <summary>
	/// 初始化内部 <see cref="AchievementService"/> 对象。
	/// </summary>
	private void InitializeInnerService()
	{
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		if (InnerService != null)
		{
			return;
		}

		var options = new AchievementServiceOptions
		{
			ClientId = Configuration["Authentication:CC98:ClientId"] ?? throw new InvalidOperationException("未配置成就系统 ClientID。"),
			ClientSecret = Configuration["Authentication:CC98:ClientSecret"] ?? throw new InvalidOperationException("未配置成就系统 ClientSecret。"),
			Authority = Configuration["Authentication:CC98:Authority"],
			ApiBaseUri = Configuration["Api:BaseUri"]
		};

		InnerService = new(HttpClient, options);
	}

	/// <summary>
	/// 应用程序配置信息。
	/// </summary>
	private IConfiguration Configuration { get; }

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
	}
}