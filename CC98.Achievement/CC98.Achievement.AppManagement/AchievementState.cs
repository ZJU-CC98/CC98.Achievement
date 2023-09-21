using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CC98.Achievement;

/// <summary>
/// 定义成就项的状态。
/// </summary>
public enum AchievementState
{
	/// <summary>
	/// 通常成就。可以在成就一览中显示，即使未获得也能看到成就详细信息，计入成就统计。
	/// </summary>
	[Display(Name = "普通成就", ShortName = "普通")]
	Normal,
	/// <summary>
	/// 隐藏成就。在解锁前，只显示该成就为隐藏成就，无法看到其他详细信息。计入成就统计。建议为需要一定技巧或者创意的成就项使用该类型。
	/// </summary>
	[Display(Name = "隐藏成就", ShortName = "隐藏")]
	Hidden,
	/// <summary>
	/// 特殊成就。解锁前无法看到任何信息，也不属于隐藏成就，不计入成就统计。建议对于需要运气或有特殊要求、普通用户无法单纯通过技巧获得的项目使用该类型。
	/// </summary>
	[Display(Name = "特殊成就", ShortName = "特殊")]
	Special
}