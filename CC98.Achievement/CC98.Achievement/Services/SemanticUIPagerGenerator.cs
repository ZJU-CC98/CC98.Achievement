using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sakura.AspNetCore.Mvc;
using Sakura.AspNetCore.Mvc.Internal;

namespace CC98.Achievement.Services;

/// <summary>
/// 提供适配 SemanticUI 的翻页器效果实现。
/// </summary>
[UsedImplicitly]
public class SemanticUIPagerGenerator : IPagerHtmlGenerator
{
	/// <inheritdoc />
	public IHtmlContent GeneratePager(PagerRenderingList list, PagerGenerationContext context)
	{
		var container = new TagBuilder("div");
		container.AddCssClass("ui pagination menu");

		foreach (var item in list.Items)
		{
			var buttonContent = GenerateForSingleItem(item);
			container.InnerHtml.AppendHtml(buttonContent);
		}

		return container;
	}

	/// <summary>
	/// 生成单个分页按钮的 HTML 代码。
	/// </summary>
	/// <param name="item">单个按钮相关信息。</param>
	/// <returns><paramref name="item"/> 对应的 HTML 代码。</returns>
	private static IHtmlContent GenerateForSingleItem(PagerRenderingItem item)
	{
		var tag = new TagBuilder("a");
		tag.AddCssClass("item");
		tag.InnerHtml.SetHtmlContent(item.Content);
		tag.Attributes.Add("href", item.Link);

		switch (item.State)
		{
			case PagerRenderingItemState.Active:
				tag.AddCssClass("active");
				break;
			case PagerRenderingItemState.Disabled:
				tag.AddCssClass("disabled");
				break;
		}

		return tag;
	}
}

