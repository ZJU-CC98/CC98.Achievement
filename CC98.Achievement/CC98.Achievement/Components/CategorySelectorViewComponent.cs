using CC98.Achievement.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CC98.Achievement.Components;

/// <summary>
/// 提供分类的选择容器。
/// </summary>
public class CategorySelectorViewComponent : ViewComponent
{
	/// <summary>
	/// 初始化 <see cref="CategoryListViewComponent"/> 对象的新实例。
	/// </summary>
	/// <param name="dbContext"><see cref="AchievementDbContext"/> 服务对象。</param>
	public CategorySelectorViewComponent(AchievementDbContext dbContext)
	{
		DbContext = dbContext;
	}

	/// <summary>
	/// 数据库上下文对象。
	/// </summary>
	private AchievementDbContext DbContext { get; }

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var cancellationToken = HttpContext.RequestAborted;

		var items =
			from i in DbContext.Categories
			orderby i.DisplayName
			select i;

		return View(await items.ToArrayAsync(cancellationToken));
	}
}
