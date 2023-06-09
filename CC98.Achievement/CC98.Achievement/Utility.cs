using Microsoft.AspNetCore.Html;

using Sakura.AspNetCore;

namespace CC98.Achievement;

/// <summary>
/// 提供辅助方法。该类型为静态类型。
/// </summary>
public static class Utility
{
	/// <summary>
	/// 获取异常的根源消息。
	/// </summary>
	/// <param name="ex">异常对象。</param>
	/// <returns>表示异常根源的消息。</returns>
	public static string GetBaseMessage(this Exception ex) => ex.GetBaseException().Message;

	/// <summary>
	/// 使用动态 HTML 本地化资源添加消息的快捷方法。
	/// </summary>
	/// <param name="messageAccessor"><see cref="IOperationMessageAccessor"/> 服务对象。</param>
	/// <param name="level">消息等级。</param>
	/// <param name="title">消息标题。</param>
	/// <param name="description">消息描述。</param>
	public static void AddMessage(this IOperationMessageAccessor messageAccessor, OperationMessageLevel level,
		dynamic title, dynamic description)
	{
		OperationMessageExtensions.Add(messageAccessor, level, (IHtmlContent)title, (IHtmlContent)description);
	}
}