namespace CC98.Achievement;

/// <summary>
/// 表示一个成就的注册信息。
/// </summary>
public class AchievementItemRegisterInfo : AchievementBaseInfo
{
	/// <summary>
	/// 如果当前成就已经存在，是否清除目前的获取记录。注意：将该属性设置为 <c>true</c> 将产生较大的影响，请谨慎使用。
	/// </summary>
	public bool ClearAllRecords { get; set; }
}