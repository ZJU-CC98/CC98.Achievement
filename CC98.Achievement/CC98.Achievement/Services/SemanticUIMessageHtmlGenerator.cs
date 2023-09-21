using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Sakura.AspNetCore;
using Sakura.AspNetCore.Mvc;

namespace CC98.Achievement.Services;

/// <summary>
/// 提供 SemanticUI 风格的消息操作提示。
/// </summary>
public class SemanticUIMessageHtmlGenerator : IOperationMessageHtmlGenerator
{
	/// <summary>
	/// 初始化 <see cref="SemanticUIMessageHtmlGenerator"/> 对象的新实例。
	/// </summary>
	/// <param name="levelClassMapper"><see cref="IOperationMessageLevelClassMapper"/> 服务对象。</param>
	public SemanticUIMessageHtmlGenerator(IOperationMessageLevelClassMapper levelClassMapper)
	{
		LevelClassMapper = levelClassMapper;
	}

	/// <summary>
	/// 消息等级到样式的映射程序。
	/// </summary>
	private IOperationMessageLevelClassMapper LevelClassMapper { get; }

	/// <inheritdoc />
	public IHtmlContent GenerateList(IEnumerable<OperationMessage> messages, MessageListStyle listStyle, bool useTwoLineMode)
	{
		var content = new HtmlContentBuilder();
		foreach (var message in messages)
		{
			// ReSharper disable once MustUseReturnValue
			content.AppendHtml(GenerateMessageItem(message, listStyle));
		}

		return content;
	}

	private IHtmlContent GenerateMessageItem(OperationMessage message, MessageListStyle listStyle)
	{
		var tag = new TagBuilder("div");
		tag.AddCssClass("ui message");

		// 添加和消息等级相关的样式、
		var levelClass = LevelClassMapper.MapLevel(message.Level, listStyle);
		tag.AddCssClass(levelClass);

		if (listStyle == MessageListStyle.AlertDialogClosable)
		{
			var closeIcon = new TagBuilder("i");
			closeIcon.AddCssClass("close item");

			tag.InnerHtml.AppendHtml(closeIcon);
		}

		var header = new TagBuilder("div");
		header.InnerHtml.SetHtmlContent(message.Title);

		var content = new TagBuilder("p");
		content.InnerHtml.SetHtmlContent(message.Description);

		tag.InnerHtml.AppendHtml(header);
		tag.InnerHtml.AppendHtml(content);

		return tag;
	}
}

/// <summary>
/// 提供针对 SemanticUI 的不同等级的消息框样式。
/// </summary>
public class SemanticUIMessageLevelClassMapper : IOperationMessageLevelClassMapper
{
	/// <inheritdoc />
	public string MapLevel(OperationMessageLevel value, MessageListStyle listStyle) =>
		value switch
		{
			OperationMessageLevel.Critical or OperationMessageLevel.Error => "negative",
			OperationMessageLevel.Warning => "warning",
			OperationMessageLevel.Success => "positive",
			OperationMessageLevel.Info => "info",
			_ => string.Empty
		};
}
