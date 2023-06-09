using CC98.Achievement.Data;

namespace CC98.Achievement.Models;

/// <summary>
/// 用户成就记录和用户详细信息。
/// </summary>
public class RecordAndUserInfo
{
	/// <summary>
	/// 成就记录。
	/// </summary>
	public AchievementRecord Record { get; set; } = null!;

	/// <summary>
	/// 用户信息。
	/// </summary>
	public User User { get; set; } = null!;
}