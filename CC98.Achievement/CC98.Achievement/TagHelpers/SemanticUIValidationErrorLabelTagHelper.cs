using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CC98.Achievement.TagHelpers;

/// <summary>
/// 为 SemanticUI 的表单隐藏提供帮助。
/// </summary>
[HtmlTargetElement("a", Attributes = ValidationForAttributeName)]
public class SemanticUIValidationErrorLabelTagHelper : TagHelper
{
	/// <summary>
	/// <see cref="ValidationFor"/> 关联到的 HTML 属性名称。该字段为常量。
	/// </summary>
	private const string ValidationForAttributeName = "asp-validation-for";


	/// <inheritdoc />
	public SemanticUIValidationErrorLabelTagHelper(IHtmlHelper htmlHelper)
	{
		HtmlHelper = htmlHelper;
	}

	/// <summary>
	/// 视图上下文信息。
	/// </summary>
	[HtmlAttributeNotBound]
	[ViewContext]
	public ViewContext ViewContext { get; set; } = null!;

	/// <summary>
	/// 要验证的数据。
	/// </summary>
	[HtmlAttributeName(ValidationForAttributeName)]
	public ModelExpression ValidationFor { get; set; } = null!;

	/// <summary>
	/// HTML 帮助程序。
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
		var fullName = HtmlHelper.Name(ValidationFor.Name);
		var modelState = ViewContext.ModelState[fullName];

		// 验证状态，无验证信息被视为没有验证状态
		var validationState = modelState?.ValidationState ?? ModelValidationState.Unvalidated;

		switch (validationState)
		{
			case ModelValidationState.Invalid:
				output.Content.SetContent(modelState!.Errors.GetFirstErrorMessage());
				output.AddClass("visible", HtmlEncoder.Default);
				break;
			default:
				output.AddClass("hidden", HtmlEncoder.Default);
				break;
		}
	}
}