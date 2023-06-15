using CC98.Achievement.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace CC98.Achievement.Components;

/// <summary>
/// 提供应用的选择界面。
/// </summary>
public class AppSelectorViewComponent : ViewComponent
{
	/// <summary>
	/// 初始化 <see cref="AppSelectorViewComponent"/> 对象的新实例。
	/// </summary>
	/// <param name="dbContext"><see cref="AchievementDbContext"/> 服务对象。</param>
	public AppSelectorViewComponent(AchievementDbContext dbContext)
	{
		DbContext = dbContext;
	}

	/// <summary>
	/// 数据库上下文对象。
	/// </summary>
	private AchievementDbContext DbContext { get; }

	/// <summary>
	/// 执行标记帮助器以显示界面。
	/// </summary>
	/// <param name="aspFor">应用选择器对应的 asp-for 绑定字段。</param>
	/// <param name="readOnly">是否只读（不可选择）。</param>
	/// <returns>表示异步操作的任务。操作结果为视图响应。</returns>
	public async Task<IViewComponentResult> InvokeAsync(ModelExpression aspFor, bool readOnly)
	{
		var cancellationToken = HttpContext.RequestAborted;

		var items =
			from i in DbContext.Categories
			orderby i.DisplayName
			select i;


		ViewBag.AspFor = aspFor;
		ViewBag.ReadOnly = readOnly;
		return View(await items.ToArrayAsync(cancellationToken));
	}
}