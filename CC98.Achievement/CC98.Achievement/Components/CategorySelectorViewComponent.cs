using CC98.Achievement.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace CC98.Achievement.Components;

/// <summary>
/// 提供分类的选择容器。
/// </summary>
/// <param name="dbContext">数据库上下文对象。</param>
public class CategorySelectorViewComponent(AchievementDbContext dbContext) : ViewComponent
{
	/// <summary>
	/// 调用视图组件。
	/// </summary>
	/// <param name="aspFor">选择器要关联到的模型表达式对象。</param>
	/// <param name="allowNull">是否允许不选择任何项目。</param>
	/// <returns>操作结果。</returns>
	public async Task<IViewComponentResult> InvokeAsync(ModelExpression aspFor, bool allowNull)
	{
		var cancellationToken = HttpContext.RequestAborted;

		var items =
			from i in dbContext.Categories.AsNoTracking()
			orderby i.DisplayName
			select i;

		ViewBag.AspFor = aspFor;
		ViewBag.AllowNull = allowNull;
		return View(await items.ToArrayAsync(cancellationToken));
	}
}
