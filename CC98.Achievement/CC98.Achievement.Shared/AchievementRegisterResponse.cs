namespace CC98.Achievement;

/// <summary>
/// 成就注册完成后，向客户端反馈的注册情况。
/// </summary>
public class AchievementRegisterResponse
{
	/// <summary>
	/// 新添加的项目数量。
	/// </summary>
	public int NewItemCount { get; set; }

	/// <summary>
	/// 更新的项目数量。
	/// </summary>
	public int UpdatedItemCount { get; set; }

	/// <summary>
	/// 删除的项目数量。
	/// </summary>
	public int DeletedItemCount { get; set; }

	/// <summary>
	/// 错误的项目数量。
	/// </summary>
	public int ErrorItemCount { get; set; }
}