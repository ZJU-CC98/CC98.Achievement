using CC98.Achievement.Data;

using System.ComponentModel.DataAnnotations;

namespace CC98.Achievement;

/// <summary>
/// 系统设置。
/// </summary>
public class SystemSetting : IAppSettingWithDefaultValue<SystemSetting>
{
	/// <summary>
	/// 默认成就图标的路径。
	/// </summary>
	[Url]
	public string DefaultIconUri { get; set; } = null!;

	/// <summary>
	/// 隐藏成就的模板信息。
	/// </summary>
	public AchievementBaseInfo HiddenItemTemplate { get; set; } = new AchievementItem();

	/// <summary>
	/// 隐藏成就的显示部分。
	/// </summary>
	public AchievementItemParts HiddenDisplayParts { get; set; } = AchievementItemParts.None;

	/// <summary>
	/// 创建并获取一个默认设置。
	/// </summary>
	/// <returns>默认设置数据。</returns>
	public static SystemSetting Default => new();
}