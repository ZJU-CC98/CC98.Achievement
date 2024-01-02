using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using CC98.Achievement.Data;
using CC98.Achievement.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Sakura.AspNetCore;
using Sakura.AspNetCore.Localization;

namespace CC98.Achievement.Controllers;

/// <summary>
/// 提供对成就的管理功能。
/// </summary>
/// <param name="dbContext">数据库上下文对象。</param>
/// <param name="messageAccessor">消息管理对象。</param>
/// <param name="sharedResourcesLocalizer">共享的本地化资源。</param>
/// <param name="localizer">本地化资源。</param>
/// <param name="dataProtectionProvider">数据保护服务。</param>
public partial class AchievementController(AchievementDbContext dbContext, IOperationMessageAccessor messageAccessor, IDynamicHtmlLocalizer<SharedResources> sharedResourcesLocalizer, IDynamicHtmlLocalizer<AchievementController> localizer, IDataProtectionProvider dataProtectionProvider) : Controller
{

	/// <summary>
	/// 显示成就系统主页。
	/// </summary>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>操作结果。</returns>
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
	{
		// 当前用户名
		var userName = User.Identity?.Name;

		// 用户未登录
		if (string.IsNullOrEmpty(userName))
		{
			// 去掉所有特殊成就
			var countItems =
				from i in dbContext.Items.AsNoTracking()
				where i.State != AchievementState.Special
				select i;

			//
			var items =
				from c in dbContext.Categories.AsNoTracking()
				join i in countItems
					on c.CodeName equals i.CategoryName into g
				select new CategorySummaryInfo
				{
					Item = c,
					VisibleAchievementCount = g.Count()

				};

			return View("IndexNoUser", await items.ToArrayAsync(cancellationToken));
		}
		else
		{
			// 和当前用户有关的记录
			var userRecords =
				from r in dbContext.Records
				where r.UserName == userName
				select r;

			// 为每个成就项生成三个数值：总数计分，普通完成计分，特殊完成计分
			// 注意总数是所所有非特殊成就的个数总和，因此非特殊成就会设置为 1
			var userAchievementCount =
				from a in dbContext.Items.AsNoTracking()
				join r in userRecords
					on new { a.CategoryName, a.CodeName } equals new { r.CategoryName, CodeName = r.AchievementName }
					into gr
				from r in gr.DefaultIfEmpty()
				select new
				{
					a.CategoryName,
					NormalFinished = r.IsCompleted && a.State != AchievementState.Special ? 1 : 0,
					SpecialFinished = r.IsCompleted && a.State == AchievementState.Special ? 1 : 0,
					VisibleCount = a.State != AchievementState.Special ? 1 : 0,
				};

			// 分类汇总求取当前用户每个分类下的普通完成数，特殊完成数和总计数
			var categoryStat =
				from i in userAchievementCount
				group i by i.CategoryName
				into g
				select new
				{
					CategoryName = g.Key,
					NormalCount = (int?)g.Sum(x => x.NormalFinished),
					SpecialCount = (int?)g.Sum(x => x.SpecialFinished),
					VisibleCount = (int?)g.Sum(x => x.VisibleCount)
				};


			var result =
				from c in dbContext.Categories.AsNoTracking()
				join i in categoryStat
					on c.CodeName equals i.CategoryName into us
				from i in us.DefaultIfEmpty()
				select new CategoryUserSummaryInfo
				{
					Item = c,
					VisibleAchievementCount = i.VisibleCount ?? 0,
					UserFinishedCount = i.NormalCount ?? 0,
					UserSpecialCount = i.SpecialCount ?? 0
				};

			return View("IndexUser", await result.ToArrayAsync(cancellationToken));
		}
	}

	///// <summary>
	///// 展示成就系统主页。
	///// </summary>
	///// <param name="category">如果该参数不为 <c>null</c> 则用户表示选中了特定分类。</param>
	///// <param name="page">要显示的页码。</param>
	///// <param name="cancellationToken">用于取消操作的令牌。</param>
	///// <returns>操作结果。</returns>
	//public async Task<IActionResult> Index(string? category = null, int mode = 0, int page = 1,
	//	CancellationToken cancellationToken = default)
	//{
	//	var userName = User.Identity?.Name;

	//	var items =
	//		from i in DbContext.Items
	//		select i;

	//	// 用户左边筛选了分类
	//	if (category != null)
	//	{
	//		items = from i in items
	//				where i.CategoryName == category
	//				select i;
	//	}

	//	// 筛选出当前用户的所有记录
	//	var userRecords =
	//		from r in DbContext.Records
	//		where r.UserName == userName
	//		select r;

	//	// 跳过所有用户未完成的特殊成
	//	// 隐藏成就在此处仍然包括，显示部分进行隐藏处理
	//	var result =
	//		from i in items.Include(p => p.Category)
	//		join r in userRecords
	//			on new { i.CategoryName, i.CodeName } equals new { r.CategoryName, CodeName = r.AchievementName } into
	//			rs
	//		from r in rs.DefaultIfEmpty()
	//		where r.IsCompleted || i.State != AchievementState.Special
	//		orderby i.CategoryName, i.SortOrder
	//		select new AchievementAndUserRecordInfo
	//		{
	//			Item = i,
	//			Record = r
	//		};

	//	var view = mode == 0 ? "Index" : "Index2";

	//	ViewBag.Category = category!;
	//	return View(view, await result.ToPagedListAsync(12, page, cancellationToken));
	//}

	/// <summary>
	/// 列出特定分类的成就一览。
	/// </summary>
	/// <param name="category">分类名称。</param>
	/// <param name="search">搜索相关设置。</param>
	/// <param name="page">页码。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>操作结果。</returns>
	[Route("[controller]/[action]/{category}")]
	public async Task<IActionResult> List(string category, UserSearchModel search, int page = 1, CancellationToken cancellationToken = default)
	{
		// 检索分类信息
		var categoryData =
			await (from i in dbContext.Categories.AsNoTracking()
				   where i.CodeName == category
				   select i).SingleOrDefaultAsync(cancellationToken);

		// 分类名不存在
		if (categoryData == null)
		{
			return NotFound();
		}

		// 附加到视图数据
		ViewBag.Category = categoryData;
		ViewBag.Search = search;

		var userName = User.Identity?.Name;

		// 所有合格的成就项目
		var items =
			from i in dbContext.Items
			where i.CategoryName == category
			select i;

		// 如果指定了类型，则筛选类型
		if (search.State != null)
		{
			items = from i in items
					where i.State == search.State
					select i;
		}

		// 筛选出当前用户的所有记录
		var userRecords =
			from r in dbContext.Records
			where r.UserName == userName
			select r;

		// 跳过所有用户未完成的特殊成就
		// 隐藏成就在此处仍然包括，显示部分进行隐藏处理
		var itemWithRecord =
			from i in items
			join r in userRecords
				on new { i.CategoryName, i.CodeName } equals new { r.CategoryName, CodeName = r.AchievementName } into
				rs
			from r in rs.DefaultIfEmpty()
			where r.IsCompleted || i.State != AchievementState.Special
			select new { Item = i, Record = r };

		// 完成状态判断
		if (search.CompleteState != null)
		{
			switch (search.CompleteState.Value)
			{
				case AchievementCompleteState.None:
					itemWithRecord = itemWithRecord.Where(i => i.Record.AchievementName == null!);
					break;
				case AchievementCompleteState.Progress:
					itemWithRecord = itemWithRecord.Where(i => i.Record.AchievementName != null! && !i.Record.IsCompleted);
					break;
				case AchievementCompleteState.Completed:
					itemWithRecord = itemWithRecord.Where(i => i.Record.IsCompleted);
					break;
			}
		}

		// 名字判断
		if (!string.IsNullOrEmpty(search.Keyword))
		{
			itemWithRecord =
				from i in itemWithRecord
				let isHidden = i.Item.State != AchievementState.Normal && !i.Record.IsCompleted
				where !isHidden && i.Item.DisplayName.Contains(search.Keyword)
				select i;
		}

		// 最终结果
		var result =
			from i in itemWithRecord

			let sortHint =
				i.Item.State == AchievementState.Normal
					? 100
					: Convert.ToInt32(i.Record.IsCompleted)
			orderby sortHint descending, i.Item.SortOrder // 先显示完成的
			select new AchievementAndUserRecordInfo
			{
				Item = i.Item,
				Record = i.Record,
			};

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
			dbContext.Items.Add(model);

			try
			{
				await dbContext.SaveChangesAsync(cancellationToken);
				Utility.AddMessage(messageAccessor, OperationMessageLevel.Success, sharedResourcesLocalizer.Html.OperationSucceeded, localizer.Html.AchievementCreated(model.DisplayName, GetUrl(model)));

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
		var item = await dbContext.Items.FindAsync([category, name], cancellationToken);
		if (item == null)
		{
			return NotFound();
		}

		var userRecords =
			await (from i in dbContext.Records
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
			dbContext.Update(model);

			try
			{
				await dbContext.SaveChangesAsync(cancellationToken);
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
			from i in dbContext.Items
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
	[HttpGet("[controller]/[action]/{category}/{name}")]
	public async Task<IActionResult> Detail(string category, string name, int userPage = 1, CancellationToken cancellationToken = default)
	{

		// 对名称解除保护
		var dataProtector = dataProtectionProvider.CreateProtector(nameof(AchievementDbContext), category.ToLowerInvariant());
		var realName = dataProtector.Unprotect(name);

		var item = await dbContext.Items.FindAsync([category, realName], cancellationToken);

		if (item == null)
		{
			return NotFound();
		}

		var userRecords =
			await (from r in dbContext.Records
				   join u in dbContext.Users
					   on EF.Functions.Collate(r.UserName, "Chinese_PRC_CI_AS") equals u.Name
				   where r.IsCompleted && r.CategoryName == category && r.AchievementName == realName
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
			dbContext.Records.Add(model);

			try
			{
				await dbContext.SaveChangesAsync(cancellationToken);
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
		var item = await dbContext.Records.FindAsync([category, name, userName], cancellationToken);

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
			dbContext.Records.Update(model);
			try
			{
				await dbContext.SaveChangesAsync(cancellationToken);
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
		var item = await dbContext.Records.FindAsync([category, name, userName], cancellationToken);

		if (item == null)
		{
			return NotFound();
		}

		dbContext.Records.Remove(item);

		try
		{
			await dbContext.SaveChangesAsync(cancellationToken);
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
			from i in dbContext.Records
			where i.CategoryName == category && i.AchievementName == name
			select i;

		dbContext.Records.RemoveRange(items);

		try
		{
			await dbContext.SaveChangesAsync(cancellationToken);
			Utility.AddMessage(messageAccessor, OperationMessageLevel.Success, sharedResourcesLocalizer.Html.OperationSucceeded, localizer.Html.RecordCleared);
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