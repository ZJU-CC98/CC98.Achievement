using CC98.Achievement.Data;
using CC98.Achievement.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CC98.Achievement.Components;

/// <summary>
/// 列出成就分类列表的视图组件。
/// </summary>
public class CategoryListViewComponent : ViewComponent
{
	/// <summary>
	/// 初始化 <see cref="CategoryListViewComponent"/> 对象的新实例。
	/// </summary>
	/// <param name="dbContext"><see cref="AchievementDbContext"/> 服务对象。</param>
	public CategoryListViewComponent(AchievementDbContext dbContext)
	{
		DbContext = dbContext;
	}

	/// <summary>
	/// 数据库上下文对象。
	/// </summary>
	private AchievementDbContext DbContext { get; }

	/// <summary>
	/// 呈现组件时调用该方法。
	/// </summary>
	/// <param name="userName">数据相关的用户名。如果为 <c>null</c> 则显示所有用户的统计信息。</param>
	/// <param name="category">要显示的分类标识。如果为 <c>null</c> 则显示未分类项目。</param>
	/// <returns>表示异步操作的任务。</returns>
	public async Task<IViewComponentResult> InvokeAsync(string? userName, string? category)
	{
		var cancellationToken = HttpContext.RequestAborted;

		ViewBag.Category = category!;

		// 去掉所有特殊成就
		if (string.IsNullOrEmpty(userName))
		{
			var countItems =
				from i in DbContext.Items
				where i.State != AchievementState.Special
				select i;

			var items =
			   from c in DbContext.Categories
			   join i in countItems
				   on c.CodeName equals i.CategoryName into g
			   select new CategorySummaryInfo
			   {
				   CodeName = c.CodeName,
				   DisplayName = c.DisplayName,
				   UserCount = c.UserCount,
				   AchievementCount = g.Count()

			   };

			return View("NoUser", await items.ToArrayAsync(cancellationToken));
		}
		else
		{
			// 和当前用户有关的记录
			var userRecords =
				from r in DbContext.Records
				where r.UserName == userName
				select r;

			// 为每个成就项生成三个数值：总数计分，普通完成计分，特殊完成计分
			// 注意总数是所所有非特殊成就的个数总和，因此非特殊成就会设置为 1
			var userAchievementCount =
				from a in DbContext.Items
				join r in userRecords
					on new { a.CategoryName, a.CodeName } equals new { r.CategoryName, CodeName = r.AchievementName } into gr
				from r in gr.DefaultIfEmpty()
				select new
				{
					a.CategoryName,
					NormalFinished = r.IsCompleted && a.State != AchievementState.Special ? 1 : 0,
					SpecialFinished = r.IsCompleted && a.State == AchievementState.Special ? 1 : 0,
					TotalCount = a.State != AchievementState.Special ? 1 : 0,
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
					TotalCount = (int?)g.Sum(x => x.TotalCount)
				};


			var result = from c in DbContext.Categories
						 join i in categoryStat
							 on c.CodeName equals i.CategoryName into us
						 from i in us.DefaultIfEmpty()
						 select new CategoryUserSummaryInfo
						 {
							 CodeName = c.CodeName,
							 DisplayName = c.DisplayName,
							 UserCount = c.UserCount,
							 AchievementCount = i.TotalCount ?? 0,
							 UserFinishedCount = i.NormalCount ?? 0,
							 UserSpecialCount = i.SpecialCount ?? 0
						 };

			return View("User", await result.ToArrayAsync(cancellationToken));
		}
	}

}