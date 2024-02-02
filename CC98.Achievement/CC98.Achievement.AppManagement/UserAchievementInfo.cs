using System;

namespace CC98.Achievement;

/// <summary>
/// 表示单个用户的一个或多个成就的信息。
/// </summary>
public class UserAchievementListInfo
{
	/// <summary>
	/// 用户名。
	/// </summary>
	public required string UserName { get; set; }

	/// <summary>
	/// 用户相关的多个成就的信息。
	/// </summary>
	public required UserAchievementItemInfo[] Items { get; set; } = Array.Empty<UserAchievementItemInfo>();

	/// <summary>
	/// 是否强制覆盖当前的成就数值。如果该属性为 <c>false</c>，则当成就记录值比更新值更高时将忽略更新。
	/// </summary>
	public bool ForceOverride { get; set; } = false;
}

/// <summary>
/// 表示用户的单个成就的相关信息。
/// </summary>
public class UserAchievementItemInfo
{
	/// <summary>
	/// 成就的代码名称。
	/// </summary>
	public required string CodeName { get; set; }

	/// <summary>
	/// 成就的当前状态值。
	/// </summary>
	public int? Value { get; set; }

	/// <summary>
	/// 成就是否完成。
	/// </summary>
	public bool IsCompleted { get; set; }
}