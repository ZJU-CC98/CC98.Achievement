using System.ComponentModel.DataAnnotations;

namespace CC98.Achievement;

/// <summary>
/// 表示一个分类的信息。
/// </summary>
public class CategoryInfo
{
	/// <summary>
	/// 分类的显示名称。用户将在成就一览页面中看到该名称。通常情况下，分类名称应当和应用名称保持一致。
	/// </summary>
	[Required] public string DisplayName { get; set; } = null!;

	/// <summary>
	/// 该分类使用的默认图标。如果设置为 <c>null</c>，则使用系统全局默认图标。
	/// </summary>
	[Url]
	public string? DefaultIconUri { get; set; }

	/// <summary>
	/// 该分类使用的默认隐藏图标。如果设置为 <c>null</c>，则使用系统全局隐藏模板定义的图标。
	/// </summary>
	[Url]
	public string? DefaultHideIconUri { get; set; }

	/// <summary>
	/// 该分类使用的应用图标，会显示在成就系统主页。
	/// </summary>
	[Url]
	public string? AppIconUri { get; set; }

	/// <summary>
	/// 分类的参与用户数量。此信息将用于统计该分类下所有成就的获得率数据。如果设定为零则将禁用获得率功能。该信息由应用自行定期更新，应当和实际使用过应用的人数保持一致。
	/// </summary>
	[Range(0, int.MaxValue)]
	public int UserCount { get; set; }
}