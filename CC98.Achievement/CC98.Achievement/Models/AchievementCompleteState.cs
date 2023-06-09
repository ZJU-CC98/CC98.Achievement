namespace CC98.Achievement.Models;

/// <summary>
/// 定义成就的完成状态。
/// </summary>
public enum AchievementCompleteState
{
	/// <summary>
	/// 未完成。
	/// </summary>
	None,
	/// <summary>
	/// 进度未完全完成。
	/// </summary>
	Progress,
	/// <summary>
	/// 已完成。
	/// </summary>
	Completed
}