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
		messageAccessor.Add(level, (IHtmlContent)title, (IHtmlContent)description);
	}

	/// <summary>
	/// 如果 <paramref name="source"/> 不为 <c>null</c>，将其变换为给定的类型；否则返回 <c>null</c>。
	/// </summary>
	/// <typeparam name="TSource">变换前的数据类型。</typeparam>
	/// <typeparam name="TResult">变换后的数据类型。</typeparam>
	/// <param name="source">要变换的数据。</param>
	/// <param name="selector">用于变换数据的方法。</param>
	/// <returns>如果 <paramref name="source"/> 不为 <c>null</c>，则返回使用 <paramref name="selector"/> 变换后的结果；否则返回 <c>null</c>。</returns>
	public static TResult? SelectIfNotNull<TSource, TResult>(this TSource? source, Func<TSource, TResult> selector)
	where TSource : class
	where TResult : class
		=> source != null ? selector(source) : null;
}