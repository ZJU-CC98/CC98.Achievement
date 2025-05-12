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
	public required UserAchievementItemInfo[] Items { get; set; } = [];

	/// <summary>
	/// 是否强制覆盖当前的成就数值。如果该属性为 <c>false</c>，则当成就记录值比更新值更高时将忽略更新。
	/// </summary>
	public bool ForceOverride { get; set; } = false;
}