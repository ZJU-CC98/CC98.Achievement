using CC98.Achievement.Data;

namespace CC98.Achievement.Models;

/// <summary>
/// 用户信息和单个成就数据。
/// </summary>
public class AchievementAndUserRecordInfo
{
	/// <summary>
	/// 成就数据。
	/// </summary>
	public AchievementItem Item { get; set; } = null!;

	/// <summary>
	/// 成就关联的用户记录。如果用户未登录或者未获得成就则为 <c>null</c>。
	/// </summary>
	public AchievementRecord? Record { get; set; }

	/// <summary>
	/// 获取当前成就的完成状态。
	/// </summary>
	public AchievementCompleteState CompleteState =>
		// 无记录
		Record == null ? AchievementCompleteState.None :
		Record.IsCompleted ? AchievementCompleteState.Completed : AchievementCompleteState.Progress;
}