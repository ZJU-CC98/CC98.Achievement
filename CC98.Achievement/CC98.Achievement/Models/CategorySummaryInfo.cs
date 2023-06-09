namespace CC98.Achievement.Models;

/// <summary>
/// 分类列表中的分类总体数据。
/// </summary>
public class CategorySummaryInfo
{
	/// <summary>
	/// 分类的名称。
	/// </summary>
	public required string DisplayName { get; set; }

	/// <summary>
	/// 分类的标识。
	/// </summary>
	public string? CodeName { get; set; }

	/// <summary>
	/// 分类下的用户数。
	/// </summary>
	public int UserCount { get; set; }

	/// <summary>
	/// 分类下的成就总数（不包含特殊成就）。
	/// </summary>
	public int AchievementCount { get; set; }
}