using CC98.Achievement.Data;
using CC98.Achievement.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Sakura.AspNetCore;
using Sakura.AspNetCore.Localization;

namespace CC98.Achievement.Controllers;

/// <summary>
/// 提供对成就的管理功能。
/// </summary>
public class AchievementController : Controller
{
	/// <summary>
	/// 初始化 <see cref="AchievementController"/> 的新实例。
	/// </summary>
	/// <param name="dbContext"><see cref="AchievementDbContext"/> 服务对象。</param>
	/// <param name="messageAccessor"><see cref="IOperationMessageAccessor"/> 服务对象。</param>
	/// <param name="sharedResourcesLocalizer"><see cref="IDynamicHtmlLocalizer{SharedResource}"/> 服务对象。</param>
	/// <param name="localizer"><see cref="IDynamicHtmlLocalizer{AchievementController}"/> 服务对象。</param>
	public AchievementController(AchievementDbContext dbContext, IOperationMessageAccessor messageAccessor, IDynamicHtmlLocalizer<SharedResources> sharedResourcesLocalizer, IDynamicHtmlLocalizer<AchievementController> localizer)
	{
		DbContext = dbContext;
		MessageAccessor = messageAccessor;
		SharedResourcesLocalizer = sharedResourcesLocalizer;
		Localizer = localizer;
	}

	/// <summary>
	/// 数据库上下文对象。
	/// </summary>
	private AchievementDbContext DbContext { get; }

	/// <summary>
	/// 消息访问器对象。
	/// </summary>
	private IOperationMessageAccessor MessageAccessor { get; }

	/// <summary>
	/// 共享字符串资源。
	/// </summary>
	private IDynamicHtmlLocalizer<SharedResources> SharedResourcesLocalizer { get; }

	/// <summary>
	/// 控制器相关字符串资源。
	/// </summary>
	private IDynamicHtmlLocalizer<AchievementController> Localizer { get; }

	/// <summary>
	/// 展示成就系统主页。
	/// </summary>
	/// <param name="category">如果该参数不为 <c>null</c> 则用户表示选中了特定分类。</param>
	/// <param name="page">要显示的页码。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>操作结果。</returns>
	public async Task<IActionResult> Index(string? category = null, int page = 1,
		CancellationToken cancellationToken = default)
	{
		var userName = User.Identity?.Name;

		var items =
			from i in DbContext.Items
			select i;

		// 用户左边筛选了分类
		if (category != null)
		{
			items = from i in items
					where i.CategoryName == category
					select i;
		}

		// 筛选出当前用户的所有记录
		var userRecords =
			from r in DbContext.Records
			where r.UserName == userName
			select r;

		// 跳过所有用户未完成的特殊成
		// 隐藏成就在此处仍然包括，显示部分进行隐藏处理
		var result =
			from i in items
			join r in userRecords
				on new { i.CategoryName, i.CodeName } equals new { r.CategoryName, CodeName = r.AchievementName } into
				rs
			from r in rs.DefaultIfEmpty()
			where r.IsCompleted || i.State != AchievementState.Special
			orderby i.CategoryName, i.SortOrder
			select new AchievementAndUserRecordInfo
			{
				Item = i,
				Record = r
			};

		ViewBag.Category = category!;
		return View(await result.ToPagedListAsync(12, page, cancellationToken));
	}

	/// <summary>
	/// 显示添加成就页面。
	/// </summary>
	/// <returns>操作结果。</returns>
	[HttpGet]
	[Authorize(Policies.Edit)]
	public IActionResult Create()
	{
		return View();
	}

	/// <summary>
	/// 执行添加成就操作。
	/// </summary>
	/// <param name="model">数据对象。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>操作结果。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policies.Edit)]
	public async Task<IActionResult> Create(AchievementItem model, CancellationToken cancellationToken)
	{
		RemoveModelError();

		if (ModelState.IsValid)
		{
			DbContext.Items.Add(model);

			try
			{
				await DbContext.SaveChangesAsync(cancellationToken);
				Utility.AddMessage(MessageAccessor, OperationMessageLevel.Success, SharedResourcesLocalizer.Html.OperationSucceeded, Localizer.Html.AchievementCreated(model.DisplayName, GetUrl(model)));

				return RedirectToAction("Manage", "Achievement");
			}
			catch (DbUpdateException ex)
			{
				ModelState.AddModelError(string.Empty, ex.GetBaseMessage());
			}
		}

		return View(model);
	}

	/// <summary>
	/// 显示成就编辑页面。
	/// </summary>
	/// <param name="category">成就分类名称。</param>
	/// <param name="name">成就名称。</param>
	/// <param name="userPage">显示成就获得用户列表一览中的页码。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>操作结果。</returns>
	[HttpGet]
	[Authorize(Policies.Edit)]
	public async Task<IActionResult> Edit(string category, string name, int userPage = 1, CancellationToken cancellationToken = default)
	{
		var item = await DbContext.Items.FindAsync(new object[] { category, name }, cancellationToken);
		if (item == null)
		{
			return NotFound();
		}

		var userRecords =
			await (from i in DbContext.Records
				   where i.CategoryName == category && i.AchievementName == name
				   orderby i.IsCompleted descending, i.Time descending
				   select i).ToPagedListAsync(20, userPage, cancellationToken);

		ViewBag.UserRecords = userRecords;
		return View(item);
	}

	/// <summary>
	/// 执行成就编辑操作。
	/// </summary>
	/// <param name="model">数据模型。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>操作结果。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policies.Edit)]
	public async Task<IActionResult> Edit(AchievementItem model, CancellationToken cancellationToken)
	{
		RemoveModelError();

		if (ModelState.IsValid)
		{
			DbContext.Update(model);

			try
			{
				await DbContext.SaveChangesAsync(cancellationToken);
				return RedirectToAction("Manage", "Achievement");
			}
			catch (DbUpdateException ex)
			{
				ModelState.AddModelError(string.Empty, ex.GetBaseMessage());
			}
		}

		return View(model);
	}


	/// <summary>
	/// 管理成就列表。
	/// </summary>
	/// <param name="search">搜索条件。</param>
	/// <param name="page">页码。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>操作结果。</returns>
	[Authorize(Policies.Edit)]
	public async Task<IActionResult> Manage(AchievementSearchModel search, int page = 1, CancellationToken cancellationToken = default)
	{
		var items =
			from i in DbContext.Items
			select i;


		if (!string.IsNullOrEmpty(search.CategoryCodeName))
		{
			items = from i in items
					where i.CategoryName == search.CategoryCodeName
					select i;
		}

		if (!string.IsNullOrEmpty(search.CodeName))
		{
			items = from i in items
					where i.CodeName.Contains(search.CodeName)
					select i;
		}

		if (!string.IsNullOrEmpty(search.Keyword))
		{
			items = from i in items
					where i.DisplayName.Contains(search.Keyword)
					select i;
		}

		if (search.State != null)
		{
			items = from i in items
					where i.State == search.State
					select i;
		}

		var result =
			(from i in items
			 orderby i.CategoryName, i.SortOrder, i.CodeName
			 select i).Include(p => p.Category);


		ViewBag.Search = search;
		return View(await result.ToPagedListAsync(20, page, cancellationToken));

	}

	/// <summary>
	/// 查看成就详细信息。
	/// </summary>
	/// <param name="category">成就分类名称。</param>
	/// <param name="name">成就名称。</param>
	/// <param name="userPage">用户列表页码。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	[HttpGet]
	public async Task<IActionResult> Detail(string category, string name, int userPage = 1, CancellationToken cancellationToken = default)
	{
		var item = await DbContext.Items.FindAsync(new object?[] { category, name }, cancellationToken);

		if (item == null)
		{
			return NotFound();
		}

		var userRecords =
			await (from r in DbContext.Records
				   join u in DbContext.Users
					   on EF.Functions.Collate(r.UserName, "Chinese_PRC_CI_AS") equals u.Name
				   where r.IsCompleted && r.CategoryName == category && r.AchievementName == name
				   orderby r.Time descending
				   select new RecordAndUserInfo
				   {
					   Record = r,
					   User = u
				   }).ToPagedListAsync(24, userPage, cancellationToken);

		var model = new AchievementDetailInfo
		{
			Item = item,
			UserRecords = userRecords
		};

		return View(model);
	}

	/// <summary>
	/// 加载添加记录对话框内容。
	/// </summary>
	/// <returns>操作结果。</returns>
	[HttpGet]
	[Authorize(Policies.Review)]
	public IActionResult AddRecord(string categoryName, string codeName, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ViewBag.CategoryName = categoryName;
		ViewBag.AchievementName = codeName;
		return PartialView();
	}

	/// <summary>
	/// 执行添加记录操作。
	/// </summary>
	/// <param name="model">数据模型。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>操作结果。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policies.Review)]
	public async Task<IActionResult> AddRecord(AchievementRecord model, CancellationToken cancellationToken)
	{
		RemoveRecordError();

		if (ModelState.IsValid)
		{
			DbContext.Records.Add(model);

			try
			{
				await DbContext.SaveChangesAsync(cancellationToken);
				return RedirectToAction("Edit", "Achievement", new { category = model.CategoryName, name = model.AchievementName });
			}
			catch (DbUpdateException ex)
			{
				ModelState.AddModelError(string.Empty, ex.GetBaseMessage());
			}
		}

		ViewBag.CategoryName = model.CategoryName;
		ViewBag.AchievementName = model.AchievementName;

		return PartialView();

	}


	/// <summary>
	/// 显示编辑成就记录页面。
	/// </summary>
	/// <param name="category">成就分类名称。</param>
	/// <param name="name">成就名称。</param>
	/// <param name="userName">用户名。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。操作结果为响应。</returns>
	[HttpGet]
	[Authorize(Policies.Review)]
	public async Task<IActionResult> EditRecord(string category, string name, string userName, CancellationToken cancellationToken)
	{
		var item = await DbContext.Records.FindAsync(new object[] { category, name, userName }, cancellationToken);

		if (item == null)
		{
			return NotFound();
		}

		return PartialView(item);
	}

	/// <summary>
	/// 执行成就记录编辑操作。
	/// </summary>
	/// <param name="model">数据模型。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>操作结果。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policies.Review)]
	public async Task<IActionResult> EditRecord(AchievementRecord model, CancellationToken cancellationToken)
	{
		RemoveRecordError();

		if (ModelState.IsValid)
		{
			DbContext.Records.Update(model);
			try
			{
				await DbContext.SaveChangesAsync(cancellationToken);
				return RedirectToAction("Edit", "Achievement",
					new { category = model.CategoryName, name = model.AchievementName });
			}
			catch (DbUpdateException ex)
			{
				ModelState.AddModelError(string.Empty, ex.GetBaseMessage());
			}
		}

		return View(model);

	}

	/// <summary>
	/// 删除用户的成就记录。
	/// </summary>
	/// <param name="category">成就分类名称。</param>
	/// <param name="name">成就名称。</param>
	/// <param name="userName">用户名。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policies.Review)]

	public async Task<IActionResult> DeleteRecord(string category, string name, string userName,
		CancellationToken cancellationToken)
	{
		var item = await DbContext.Records.FindAsync(new object[] { category, name, userName }, cancellationToken);

		if (item == null)
		{
			return NotFound();
		}

		DbContext.Records.Remove(item);

		try
		{
			await DbContext.SaveChangesAsync(cancellationToken);
			return RedirectToAction("Edit", "Achievement", new { id = category });
		}
		catch (DbUpdateException ex)
		{
			return BadRequest(ex.GetBaseMessage());
		}
	}

	/// <summary>
	/// 清空某项成就的所有记录。
	/// </summary>
	/// <param name="category">成就的分类标识。</param>
	/// <param name="name">成就的标识。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Policies.Review)]
	public async Task<IActionResult> ClearRecords(string category, string name, CancellationToken cancellationToken = default)
	{
		var items =
			from i in DbContext.Records
			where i.CategoryName == category && i.AchievementName == name
			select i;

		DbContext.Records.RemoveRange(items);

		try
		{
			await DbContext.SaveChangesAsync(cancellationToken);
			Utility.AddMessage(MessageAccessor, OperationMessageLevel.Success, SharedResourcesLocalizer.Html.OperationSucceeded, Localizer.Html.RecordCleared);
			return RedirectToAction("Edit", "Achievement", new { category, name });
		}
		catch (DbUpdateException ex)
		{
			return BadRequest(ex.GetBaseMessage());
		}
	}

	/// <summary>
	/// 成就信息修改操作时，移除因为外键关联产生的不必要的模型错误。
	/// </summary>
	private void RemoveRecordError()
	{
		ModelState.Remove(nameof(AchievementRecord.Achievement));
	}

	/// <summary>
	/// 用户记录修改操作时，移除因为外键关联产生的不必要的模型错误。
	/// </summary>
	private void RemoveModelError()
	{
		ModelState.Remove(nameof(AchievementItem.Category));
	}

	/// <summary>
	/// 生成成就的详情 URL。
	/// </summary>
	/// <param name="item">成就对象。</param>
	/// <returns><paramref name="item"/> 对应的详细信息 URL。</returns>
	private string? GetUrl(AchievementItem item) => Url.Action("Detail", "Achievement",
		new { cateory = item.CategoryName, name = item.CodeName });
}