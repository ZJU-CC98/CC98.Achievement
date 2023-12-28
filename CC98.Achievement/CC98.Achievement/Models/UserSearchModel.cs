namespace CC98.Achievement.Models;

/// <summary>
/// 用户搜索成就时使用的数据模型。
/// </summary>
public class UserSearchModel
{
	/// <summary>
	/// 成就名称关键字。
	/// </summary>
	public string? Keyword { get; set; }

	/// <summary>
	/// 成就类型。
	/// </summary>
	public AchievementState? State { get; set; }

	/// <summary>
	/// 成就完成状态。
	/// </summary>
	public AchievementCompleteState? CompleteState { get; set; }
}