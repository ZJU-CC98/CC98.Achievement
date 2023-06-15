using System.Collections.Generic;

namespace CC98.Achievement;

/// <summary>
/// 注册成就时需要提供的数据。
/// </summary>
public class AchievementRegisterInfo
{
	/// <summary>
	/// 需要注册的成就项目的集合。
	/// </summary>
	public required IEnumerable<AchievementItemRegisterInfo> Items { get; set; }

	/// <summary>
	/// 注册时提供的相关选项。
	/// </summary>
	public required AchievementRegisterOptions Options { get; set; }
}