namespace CC98.Achievement;

/// <summary>
/// 表示注册成就时的选项。
/// </summary>
public class AchievementRegisterOptions
{
	/// <summary>
	/// 如果该属性为 <c>true</c> 则表示同时注册成就时将同时清除该应用名下已经存在但未在本次注册清单中列出的成就。
	/// </summary>
	public bool RemoveAllNonListedItems { get; set; }

	/// <summary>
	/// 如果该属性为 <c>true</c> 则将该分类中的所有成就项按照注册时提供的顺序重新排序。
	/// </summary>
	/// <remarks>
	/// 请注意，如果应用之前已经创建了一项成就，但未出现在本次的注册列表中，则它们在重新排序后它们将可能会出现在任意随机位置。
	/// </remarks>
	public bool ReorderItems { get; set; }
}