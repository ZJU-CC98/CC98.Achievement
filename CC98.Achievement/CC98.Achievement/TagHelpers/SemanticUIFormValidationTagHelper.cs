using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CC98.Achievement.TagHelpers;

/// <summary>
/// 为 SemanticUI 表单验证提供帮助。
/// </summary>
[HtmlTargetElement("div", Attributes = "asp-for")]
public class SemanticUIFormValidationTagHelper : TagHelper
{
	/// <inheritdoc />
	public SemanticUIFormValidationTagHelper(IHtmlHelper htmlHelper)
	{
		HtmlHelper = htmlHelper;
	}

	/// <summary>
	/// 该字段关联到的表达式。
	/// </summary>
	[HtmlAttributeName("asp-for")]
	public ModelExpression AspFor { get; set; } = null!;


	/// <summary>
	/// 视图上下文对象。
	/// </summary>
	[HtmlAttributeNotBound]
	[ViewContext]
	public ViewContext ViewContext { get; set; } = null!;


	/// <summary>
	/// HTML 帮助器服务。
	/// </summary>
	private IHtmlHelper HtmlHelper { get; }

	/// <inheritdoc />
	public override void Init(TagHelperContext context)
	{
		if (HtmlHelper is IViewContextAware viewContextAware)
		{
			viewContextAware.Contextualize(ViewContext);
		}
	}

	/// <inheritdoc />
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		if (AspFor.Metadata.IsRequired)
		{
			output.AddClass("required", HtmlEncoder.Default);
		}

		var modelEntry = ViewContext.ModelState[HtmlHelper.Name(AspFor.Name)];

		if (modelEntry == null)
		{
			return;
		}

		switch (modelEntry.ValidationState)
		{
			case ModelValidationState.Invalid:
				output.AddClass("error", HtmlEncoder.Default);
				break;
		}
	}
}