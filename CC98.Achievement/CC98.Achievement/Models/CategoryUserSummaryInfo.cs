namespace CC98.Achievement.Models;

/// <summary>
/// 分类列表中用户登录后的总体数据。
/// </summary>
public class CategoryUserSummaryInfo : CategorySummaryInfo
{
	/// <summary>
	/// 用户完成的成就数（不包括特殊成就。）
	/// </summary>
	public int UserFinishedCount { get; set; }

	/// <summary>
	/// 用户完成的特殊成就数。
	/// </summary>
	public int UserSpecialCount { get; set; }
}