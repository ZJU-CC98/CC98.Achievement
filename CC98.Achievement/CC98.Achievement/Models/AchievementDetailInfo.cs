using CC98.Achievement.Data;

using Sakura.AspNetCore;

namespace CC98.Achievement.Models;

/// <summary>
/// 成就详细信息页面关联的数据。。
/// </summary>
public class AchievementDetailInfo
{
	/// <summary>
	/// 成就数据。
	/// </summary>
	public AchievementItem Item { get; set; } = null!;

	/// <summary>
	/// 分页的用户列表。
	/// </summary>
	public IPagedList<RecordAndUserInfo> UserRecords { get; set; } = null!;
}