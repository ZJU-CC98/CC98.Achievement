namespace CC98.Achievement;

/// <summary>
/// 定义成就的显示部分。
/// </summary>
[Flags]
public enum AchievementItemParts
{
	/// <summary>
	/// 无任何项目。
	/// </summary>
	None = 0,
	/// <summary>
	/// 成就的名称。
	/// </summary>
	Name = 0x1,
	/// <summary>
	/// 成就的图标。
	/// </summary>
	Icon = 0x2,
	/// <summary>
	/// 成就的描述。
	/// </summary>
	Description = 0x4,
	/// <summary>
	/// 成就的提示。
	/// </summary>
	Hint = 0x8,
	/// <summary>
	/// 成就的奖励。
	/// </summary>
	Reward = 0x10,
}