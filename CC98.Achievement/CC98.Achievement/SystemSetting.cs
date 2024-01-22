using CC98.Achievement.Data;

using System.ComponentModel.DataAnnotations;

namespace CC98.Achievement;

/// <summary>
/// 系统设置。
/// </summary>
public class SystemSetting : IAppSettingWithDefaultValue<SystemSetting>
{
	/// <summary>
	/// 默认的应用图标。
	/// </summary>
	[Url] public string DefaultAppIconUri { get; set; } = null!;

	/// <summary>
	/// 默认成就图标的路径。
	/// </summary>
	[Url]
	public string DefaultIconUri { get; set; } = null!;

	/// <summary>
	/// 默认的灰色成就图标的路径。如果该属性为 <c>null</c>，则会使用 <see cref="DefaultIconUri"/> 的灰色版本作为默认灰色图标。
	/// </summary>
	[Url]
	public string? DefaultGrayedIconUri { get; set; }

	/// <summary>
	/// 隐藏成就的模板信息。
	/// </summary>
	public AchievementItem HiddenItemTemplate { get; set; } = new();

	/// <summary>
	/// 为 <see cref="HiddenItemTemplate"/> 创建特定于分类的模板。
	/// </summary>
	/// <param name="category">要关联到的分类。</param>
	/// <returns>一个 <see cref="AchievementItem"/> 对象，其中主要内容来自于 <see cref="HiddenItemTemplate"/>，但 <see cref="AchievementItem.Category"/> 属性来自于 <paramref name="category"/> 参数。</returns>
	public AchievementItem GetHiddenTemplateForCategory(AchievementCategory category)
	{
		var result = HiddenItemTemplate.Clone();
		result.Category = category;
		result.CategoryName = category.CodeName;

		// 替换为隐藏图标
		result.IconUri = category.DefaultHideIconUri;

		return result;
	}

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