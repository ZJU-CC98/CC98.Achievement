namespace CC98.Achievement;

/// <summary>
/// 定义应用系统策略。该类型为静态类型。
/// </summary>
public static class Policies
{
	/// <summary>
	/// 管理成就系统。
	/// </summary>
	public const string Admin = nameof(Admin);

	/// <summary>
	/// 向用户授予或回收成就。
	/// </summary>
	public const string Review = nameof(Review);

	/// <summary>
	/// 编辑成就信息。
	/// </summary>
	public const string Edit = nameof(Edit);

	/// <summary>
	/// 必须由第三方应用调用的 API 接口。
	/// </summary>
	public const string ClientApp = nameof(ClientApp);

	/// <summary>
	/// 定义系统角色。该类型为静态类型。
	/// </summary>
	public static class Roles
	{
		/// <summary>
		/// 标识系统管理员角色。
		/// </summary>
		public const string GeneralAdministrators = "Administrators";

		/// <summary>
		/// 成就系统管理员角色。
		/// </summary>
		public const string Administrators = "Achievement Administrators";

		/// <summary>
		/// 成就系统操作员角色。
		/// </summary>
		public const string Operators = "Achievement Operators";

		/// <summary>
		/// 成就系统审核员角色。
		/// </summary>
		public const string Reviewers = "Achivement Reviewers";

		/// <summary>
		/// 成就系统编辑员角色。
		/// </summary>
		public const string Editors = "Achievement Editors";
	}
}