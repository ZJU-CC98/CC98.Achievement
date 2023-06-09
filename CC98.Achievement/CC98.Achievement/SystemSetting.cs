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
	/// 隐藏成就图标的路径。
	/// </summary>
	[Url]
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

/// <summary>
/// 定义成就的显示部分。
/// </summary>
[Flags]
public enum AchievementItemParts
{
	/// <summary>
	/// 无任何项目。
	/// </summary>
	None = 0,
	/// <summary>
	/// 成就的名称。
	/// </summary>
	Name = 0x1,
	/// <summary>
	/// 成就的图标。
	/// </summary>
	Icon = 0x2,
	/// <summary>
	/// 成就的描述。
	/// </summary>
	Description = 0x4,
	/// <summary>
	/// 成就的提示。
	/// </summary>
	Hint = 0x8,
	/// <summary>
	/// 成就的奖励。
	/// </summary>
	Reward = 0x10,
}