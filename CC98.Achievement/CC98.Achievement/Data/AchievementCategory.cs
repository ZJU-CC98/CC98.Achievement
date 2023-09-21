using Microsoft.EntityFrameworkCore;

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.Achievement.Data;

/// <summary>
/// 表示可颁发成就的一个类别。
/// </summary>
[Index(nameof(AppId), IsUnique = true)]
[Index(nameof(DisplayName))]
public class AchievementCategory
{
	/// <summary>
	/// 该类别的代码名称。通常和应用名称相关。
	/// </summary>
	[Key]
	[StringLength(128)]
	[RegularExpression(@"^[\w_\-]+$")]
	public string CodeName { get; set; } = null!;

	/// <summary>
	/// 该应用程序的显示名称。该名称通常应当和应用程序的名称保持一致。
	/// </summary>
	[Required]
	public string DisplayName { get; set; } = null!;

	/// <summary>
	/// 该应用程序在 CC98 应用系统中定义的应用标识。
	/// </summary>
	public string? AppId { get; set; }

	/// <summary>
	/// 获取或设置该分类涉及的用户数量。
	/// </summary>
	[Range(0, int.MaxValue)]
	public int UserCount { get; set; }

	/// <summary>
	/// 该分类的应用主图标。
	/// </summary>
	[Url]
	public string? AppIconUri { get; set; } = null!;

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
	/// 获取或设置该颁发者关联的成就的集合。
	/// </summary>
	[InverseProperty(nameof(AchievementItem.Category))]
	public virtual ICollection<AchievementItem> Items { get; set; } = new Collection<AchievementItem>();
}