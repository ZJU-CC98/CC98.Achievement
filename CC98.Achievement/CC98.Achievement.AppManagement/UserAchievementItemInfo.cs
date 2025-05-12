namespace CC98.Achievement;

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