using System.ComponentModel.DataAnnotations;

namespace CC98.Achievement.Models;

/// <summary>
/// 定义成就的完成状态。
/// </summary>
public enum AchievementCompleteState
{
	/// <summary>
	/// 未完成。
	/// </summary>
	[Display(ShortName = "未获得")]
	None,
	/// <summary>
	/// 进度未完全完成。
	/// </summary>
	[Display(ShortName = "获得中")]
	Progress,
	/// <summary>
	/// 已完成。
	/// </summary>
	[Display(ShortName = "已获得")]
	Completed
}