using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.Achievement.Data;

/// <summary>
/// 表示一个成就的获得记录。
/// </summary>
[PrimaryKey(nameof(CategoryName), nameof(AchievementName), nameof(UserName))]
[Index(nameof(UserName))]
[Index(nameof(UserName), nameof(IsCompleted), nameof(Time))]
[Index(nameof(CategoryName), nameof(AchievementName), nameof(IsCompleted), nameof(Time))]
public class AchievementRecord
{
	/// <summary>
	/// 成就记录对应的成就分类的标识。该属性和 <see cref="AchievementName"/> 共同决定了关联的 <see cref="AchievementItem"/> 对象。
	/// </summary>
	[Required]
	public string CategoryName { get; set; } = null!;

	/// <summary>
	/// 成就记录对应的成就的标识。该属性和 <see cref="CategoryName"/> 共同决定了关联的 <see cref="AchievementItem"/> 对象。
	/// </summary>
	[Required]
	public string AchievementName { get; set; } = null!;

	/// <summary>
	/// 成就记录对应的用户名。
	/// </summary>
	[Required]
	[StringLength(128)]
	public string UserName { get; set; } = null!;

	/// <summary>
	/// 获取或设置该项目关联的成就。
	/// </summary>
	[ForeignKey($"{nameof(CategoryName)},{nameof(AchievementName)}")]
	public AchievementItem Achievement { get; set; } = null!;

	/// <summary>
	/// 定义成就的当前值。对于无阶段的成就，该数值没有意义。
	/// </summary>
	[Range(0, int.MaxValue)]
	public int? CurrentValue { get; set; }

	/// <summary>
	/// 获取或设置一个值，指示该成就是否完成。
	/// </summary>
	public bool IsCompleted { get; set; }

	/// <summary>
	/// 定义成就的完成时间。对于具有进度的成就，则表示最后更新时间。
	/// </summary>
	public DateTimeOffset Time { get; set; }
}