using Microsoft.EntityFrameworkCore;

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.Achievement.Data;

/// <summary>
/// 表示可颁发成就的一个类别。
/// </summary>
[Index(nameof(AppId), IsUnique = true)]
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
	/// 获取或设置该颁发者关联的成就的集合。
	/// </summary>
	[InverseProperty(nameof(AchievementItem.Category))]
	public virtual ICollection<AchievementItem> Items { get; set; } = new Collection<AchievementItem>();
}