using System.ComponentModel.DataAnnotations;

namespace CC98.Achievement;

/// <summary>
/// 定义成就的显示部分。
/// </summary>
[Flags]
public enum AchievementItemParts
{
	/// <summary>
	/// 无任何项目。
	/// </summary>
	[Display(Name = "无")]
	None = 0,
	/// <summary>
	/// 成就的名称。
	/// </summary>
	[Display(Name = "成就名称", ShortName = "名称")]
	Name = 0x1,
	/// <summary>
	/// 成就的图标。
	/// </summary>
	[Display(Name = "成就图标", ShortName = "图标")]
	Icon = 0x2,
	/// <summary>
	/// 成就的描述。
	/// </summary>
	[Display(Name = "成就描述", Description = "描述")]
	Description = 0x4,
	/// <summary>
	/// 成就的提示。
	/// </summary>
	[Display(Name = "获得提示", ShortName = "提示")]
	Hint = 0x8,
	/// <summary>
	/// 成就的奖励。
	/// </summary>
	[Display(Name = "成就奖励", ShortName = "奖励")]
	Reward = 0x10,
}