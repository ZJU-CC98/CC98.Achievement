using CC98.Achievement.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CC98.Achievement.Components;

/// <summary>
/// 提供应用的选择界面。
/// </summary>
public class AppSelectorViewComponent : ViewComponent
{
	/// <inheritdoc />
	public AppSelectorViewComponent(AchievementDbContext dbContext)
	{
		DbContext = dbContext;
	}

	private AchievementDbContext DbContext { get; }

	public async Task<IViewComponentResult> InvokeAsync(ModelExpression aspFor, bool readOnly)
	{
		var items =
			from i in DbContext.Categories
			orderby i.DisplayName
			select i;


		ViewBag.AspFor = aspFor;
		ViewBag.ReadOnly = readOnly;
		return View(items);
	}
}