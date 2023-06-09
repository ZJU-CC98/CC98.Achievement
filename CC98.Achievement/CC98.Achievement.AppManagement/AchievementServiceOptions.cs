namespace CC98.Achievement.AppManagement;

/// <summary>
/// 为 <see cref="AchievementService"/> 提供配置选项。
/// </summary>
public class AchievementServiceOptions
{
	/// <summary>
	/// 客户端标识。
	/// </summary>
	public string ClientId { get; set; } = null!;

	/// <summary>
	/// 客户端机密。
	/// </summary>
	public string ClientSecret { get; set; } = null!;

	/// <summary>
	/// 客户端身份授权服务机构地址。
	/// </summary>
	public string? Authority { get; set; } = AchievementServiceDefaults.Authority;

	/// <summary>
	/// API 操作的 URL 根地址。
	/// </summary>
	public string? ApiBaseUri { get; set; } = AchievementServiceDefaults.ApiBaseUri;
}