using Microsoft.EntityFrameworkCore;

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.Achievement.Data;

/// <summary>
/// 定义一项成就。
/// </summary>
[PrimaryKey(nameof(CategoryName), nameof(CodeName))]
[Index(nameof(CategoryName), nameof(SortOrder))]
public class AchievementItem : AchievementBaseInfo
{
	/// <summary>
	/// 获取或设置该成就的排序顺序。
	/// </summary>
	public int SortOrder { get; set; }

	/// <summary>
	/// 获取或设置该成就的颁发者应用的标识。
	/// </summary>
	public string CategoryName { get; set; } = null!;

	/// <summary>
	/// 获取或设置该成就的颁发者应用。
	/// </summary>
	[ForeignKey(nameof(CategoryName))]
	public AchievementCategory Category { get; set; } = null!;

	/// <summary>
	/// 获取或设置该成就对应的用户记录的集合。
	/// </summary>
	[InverseProperty(nameof(AchievementRecord.Achievement))]
	public virtual ICollection<AchievementRecord> Records { get; set; } = new Collection<AchievementRecord>();
}