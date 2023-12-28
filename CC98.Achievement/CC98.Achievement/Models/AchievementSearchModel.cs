namespace CC98.Achievement.Models;

/// <summary>
/// 定义成就的搜索模型。
/// </summary>
public class AchievementSearchModel
{
	/// <summary>
	/// 分类代码名称。
	/// </summary>
	public string? CategoryCodeName { get; set; }

	/// <summary>
	/// 成就代码名称。
	/// </summary>
	public string? CodeName { get; set; }

	/// <summary>
	/// 名称关键字。
	/// </summary>
	public string? Keyword { get; set; }

	/// <summary>
	/// 成就类型。
	/// </summary>
	public AchievementState? State { get; set; }
}