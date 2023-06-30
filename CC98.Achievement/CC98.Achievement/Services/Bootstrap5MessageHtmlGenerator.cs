using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Sakura.AspNetCore;
using Sakura.AspNetCore.Localization;
using Sakura.AspNetCore.Mvc;

namespace CC98.Achievement.Services;

/// <summary>
/// 提供 Bootstrap 5 风格的消息列表。
/// </summary>
public class Bootstrap5MessageHtmlGenerator : IOperationMessageHtmlGenerator
{
	/// <summary>
	/// 初始化 <see cref="Bootstrap5MessageHtmlGenerator"/> 对象的新实例。
	/// </summary>
	/// <param name="levelClassMapper"><see cref="IOperationMessageLevelClassMapper"/> 服务对象。</param>
	/// <param name="sharedResourcesLocalizer"><see cref="IDynamicHtmlLocalizer{SharedResources}"/> 服务对象。</param>
	public Bootstrap5MessageHtmlGenerator(IOperationMessageLevelClassMapper levelClassMapper, IDynamicHtmlLocalizer<SharedResources> sharedResourcesLocalizer)
	{
		LevelClassMapper = levelClassMapper;
		SharedResourcesLocalizer = sharedResourcesLocalizer;
	}

	/// <summary>
	/// 提供将消息警告级别映射到对应 CSS 样式类的服务。
	/// </summary>
	private IOperationMessageLevelClassMapper LevelClassMapper { get; }

	/// <summary>
	/// 为生成的文本内容提供本地化支持。
	/// </summary>
	private IDynamicHtmlLocalizer<SharedResources> SharedResourcesLocalizer { get; }

	/// <inheritdoc />
	public IHtmlContent GenerateList(IEnumerable<OperationMessage> messages, MessageListStyle listStyle, bool useTwoLineMode)
	{
		var builder = new HtmlContentBuilder();

		foreach (var item in messages)
		{
			// ReSharper disable once MustUseReturnValue
			builder.AppendHtml(GenerateItem(item, listStyle, useTwoLineMode));
		}

		return builder;
	}

	private IHtmlContent GenerateItem(OperationMessage message, MessageListStyle style, bool useTwoLineMode)
	{
		var title = new TagBuilder("strong");
		title.InnerHtml.AppendHtml(message.Title);

		var body = new TagBuilder("span");
		body.InnerHtml.AppendHtml(message.Description);

		var container = new TagBuilder("div");
		container.AddCssClass("alert");
		container.AddCssClass(LevelClassMapper.MapLevel(message.Level, style));
		container.Attributes.Add("role", "alert");

		container.InnerHtml.AppendLine(title);

		// 如果需要就插入换行
		if (useTwoLineMode)
		{
			container.InnerHtml.AppendHtml(new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing });
		}

		container.InnerHtml.AppendLine(body);

		if (style == MessageListStyle.AlertDialogClosable)
		{
			var closeButton = new TagBuilder("button");

			closeButton.MergeAttributes(new Dictionary<string, string?>
			{
				["type"] = "button",
				["data-bs-dismiss"] = "alert",
				["aria-label"] = SharedResourcesLocalizer.Text.CloseButtonText,
				["class"] = "btn-close"
			});

			container.AddCssClass("alert-dismissible fade show");
			container.InnerHtml.AppendHtml(closeButton);
		}

		return container;
	}
}