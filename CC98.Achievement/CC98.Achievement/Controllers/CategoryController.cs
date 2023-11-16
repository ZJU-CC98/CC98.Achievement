using CC98.Achievement.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Sakura.AspNetCore;
using Sakura.AspNetCore.Localization;

namespace CC98.Achievement.Controllers;

/// <summary>
/// 提供对成就系统业务应用的管理。
/// </summary>
/// <param name="dbContext">数据库上下文对象。</param>
/// <param name="messageAccessor">消息管理器。</param>
/// <param name="sharedResourcesLocalizer">共享的本地化资源。</param>
/// <param name="localizer">本地化资源。</param>
public class CategoryController(AchievementDbContext dbContext, IOperationMessageAccessor messageAccessor, IDynamicHtmlLocalizer<SharedResources> sharedResourcesLocalizer, IDynamicHtmlLocalizer<CategoryController> localizer) : Controller
{

	/// <summary>
	/// 显示管理界面。
	/// </summary>
	/// <param name="page">页码。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。操作结果包含页面响应。</returns>
	[HttpGet]
	[Authorize(Policies.Admin)]
	public async Task<IActionResult> Manage(int page = 1, CancellationToken cancellationToken = default)
	{
		var items =
			from i in dbContext.Categories
			orderby i.DisplayName
			select i;

		return View(await items.ToPagedListAsync(20, page, cancellationToken));
	}

	/// <summary>
	/// 显示创建分类页面。
	/// </summary>
	/// <returns>页面响应。</returns>
	[HttpGet]
	[Authorize(Policies.Admin)]
	public IActionResult Create()
	{
		return View();
	}

	/// <summary>
	/// 执行创建应用操作。
	/// </summary>
	/// <param name="model">数据模型。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。操作结果包含页面响应。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policies.Admin)]
	public async Task<IActionResult> Create(AchievementCategory model, CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			dbContext.Categories.Add(model);

			try
			{
				await dbContext.SaveChangesAsync(cancellationToken);
				Utility.AddMessage(messageAccessor, OperationMessageLevel.Success,
					sharedResourcesLocalizer.Html.OperationSucceeded,
					localizer.Html.CategoryCreated(model.DisplayName, GetCategoryUri(model.CodeName)));

				return RedirectToAction("Manage", "Category");
			}
			catch (DbUpdateException ex)
			{
				ModelState.AddModelError(string.Empty, ex.GetBaseMessage());
			}
		}

		return View(model);
	}


	/// <summary>
	/// 显示应用编辑界面。
	/// </summary>
	/// <param name="codeName">应用的代码名称。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。操作结果包含页面响应。</returns>
	[HttpGet]
	[Authorize(Policies.Admin)]
	public async Task<IActionResult> Edit(string codeName, CancellationToken cancellationToken)
	{
		var item = await dbContext.Categories.FindAsync([codeName], cancellationToken);

		if (item == null)
		{
			return NotFound();
		}

		return View(item);
	}

	/// <summary>
	/// 执行应用编辑已操作。
	/// </summary>
	/// <param name="model">数据模型。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。操作结果包含页面响应。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policies.Admin)]
	public async Task<IActionResult> Edit(AchievementCategory model, CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			dbContext.Update(model);

			try
			{
				await dbContext.SaveChangesAsync(cancellationToken);

				Utility.AddMessage(messageAccessor, OperationMessageLevel.Success,
					sharedResourcesLocalizer.Html.OperationSucceeded,
					localizer.Html.AppUpdated(model.DisplayName, GetCategoryUri(model.CodeName)));

				return RedirectToAction("Manage", "Category");
			}
			catch (DbUpdateException ex)
			{
				ModelState.AddModelError(string.Empty, ex.GetBaseMessage());
			}
		}

		return View(model);
	}

	/// <summary>
	/// 执行删除操作。
	/// </summary>
	/// <param name="codeName">要删除的应用的标识。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。任务结果表示操作结果。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policies.Admin)]

	public async Task<IActionResult> Delete(int codeName, CancellationToken cancellationToken)
	{
		var item = await dbContext.Categories.FindAsync([codeName], cancellationToken);

		if (item == null)
		{
			return NotFound();
		}

		try
		{
			dbContext.Categories.Remove(item);
			await dbContext.SaveChangesAsync(cancellationToken);
			Utility.AddMessage(messageAccessor, OperationMessageLevel.Success,
				sharedResourcesLocalizer.Html.OperationSucceeded, localizer.Html.AppDeleted(item.DisplayName));
		}
		catch (DbUpdateException ex)
		{
			Utility.AddMessage(messageAccessor, OperationMessageLevel.Success,
				sharedResourcesLocalizer.Html.OperationFailed, localizer.Html.AppDeleteFailed(item.DisplayName, Url.Action("Detail", "Category", new { item.CodeName }), ex.GetBaseMessage()));
		}

		return RedirectToAction("Manage", "Category");
	}

	/// <summary>
	/// 显示应用详情界面。
	/// </summary>
	/// <param name="codeName">应用的代码名称。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。任务结果表示操作结果。</returns>
	[HttpGet]
	[Authorize(Policies.Admin)]
	public async Task<IActionResult> Detail(string codeName, CancellationToken cancellationToken)
	{
		var item =
			await dbContext.Categories.FindAsync([codeName], cancellationToken);

		if (item == null)
		{
			return NotFound();
		}

		return View(item);
	}

	/// <summary>
	/// 获取应用的访问路径。该方法为辅助方法。
	/// </summary>
	/// <param name="codeName">应用标识。</param>
	/// <returns>应用的访问路径。</returns>
	private string? GetCategoryUri(string codeName) => Url.Action("Detail", "Category", new { id = codeName });
}