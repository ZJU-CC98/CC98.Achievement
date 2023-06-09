using System.ComponentModel.DataAnnotations;

namespace CC98.Achievement;

/// <summary>
/// 为数据库和操纵 API 对象提供公用基础类型。
/// </summary>
public abstract class AchievementBaseInfo
{
	/// <summary>
	/// 成就的代码名称。该字段用于在其他 API 中标识该成就。成就的代码名称设定后将无法修改。
	/// </summary>
	[StringLength(128)]
	[RegularExpression(@"^[\w_\-]+$")]

	public string CodeName { get; set; } = null!;

	/// <summary>
	/// 成就的显示名称，将会显示在成就列表和详细信息页面。
	/// </summary>
	[Required]
	public string DisplayName { get; set; } = null!;

	/// <summary>
	/// 成就的描述。将会显示在成就详细信息界面。
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// 成就的图标地址。如果不提供该信息，则将使用成就系统提供的默认图标。
	/// </summary>
	[Url]
	public string? IconUri { get; set; }

	/// <summary>
	/// 如果提供了该字段，用户将可以在成就的详细信息页面看到如何获取这项成就的提示。
	/// </summary>
	public string? Hint { get; set; }

	/// <summary>
	/// 成就的奖励内容。注意，成就奖励的颁发需应用自行处置，该字段的的内容用于显示在成就详细信息界面，不影响成就系统实际运行，也无格式要求。
	/// </summary>
	public string? Reward { get; set; }

	/// <summary>
	/// 成就的类型。有关成就类型的详细信息，请类型文档。
	/// </summary>
	public AchievementState State { get; set; }

	/// <summary>
	/// 如果提供了该值，则该成就将会变成计数成就，用户需要满足次数或数量要求才能获得成就。成就系统将负责记录数值，但数据更新由应用负责。如果未提供该数值，则本成就不启用计数。
	/// </summary>
	public int? MaxValue { get; set; }

	/// <summary>
	/// 如果该属性为 <c>true</c>，则成就信息面板将提示用户这是一个动态成就（判断条件可能随时间变化），这意味着成就获得后可能因为满足条件随实际情况变化再次失去成就。不推荐使用动态成就，这可能会降低用户获得成就的体验感。
	/// </summary>
	/// <remarks>
	/// 注意：这个属性不影响 API 操作。定义成就的应用始终可以通过 API 直接移除用户已经获得的成就。这个信息仅用于提示用户的获得风险。
	/// </remarks>
	public bool IsDynamic { get; set; }
}