using System;

namespace CC98.Achievement;

/// <summary>
/// 表示单个用户单个成就的获得状态。
/// </summary>
public class RecordInfo
{
	/// <summary>
	/// 成就关联的用户名。
	/// </summary>
	public string UserName { get; set; } = null!;

	/// <summary>
	/// 成就的代码名。
	/// </summary>
	public string AchievementCodeName { get; set; } = null!;

	/// <summary>
	/// 成绩的当前进度值。如果该成就未启用进度功能，则该数值通常为 <c>null</c>。
	/// </summary>
	public int? CurrentValue { get; set; }

	/// <summary>
	/// 成就是否已经完成。
	/// </summary>
	public bool IsCompleted { get; set; }

	/// <summary>
	/// 成就状态的最后更新时间。
	/// </summary>
	public DateTimeOffset LastUpdateTime { get; set; }
}