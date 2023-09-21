using CC98.Achievement.Data;

namespace CC98.Achievement.Models;

/// <summary>
/// 分类列表中的分类总体数据。
/// </summary>
public class CategorySummaryInfo
{

	/// <summary>
	/// 实际对应的分类信息。
	/// </summary>
	public required AchievementCategory Item { get; set; }

	/// <summary>
	/// 用户可看到的成就数量。
	/// </summary>
	public required int VisibleAchievementCount { get; set; }
}