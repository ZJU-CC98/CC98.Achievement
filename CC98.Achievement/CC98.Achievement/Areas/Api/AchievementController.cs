﻿using CC98.Achievement.Data;

using IdentityModel;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using EFCore.BulkExtensions;

namespace CC98.Achievement.Areas.Api;

/// <summary>
/// 为第三方 API 提供 RESTful 调用接口。
/// </summary>
/// <param name="dbContext">数据库上下文对象。</param>
[Area("Api")]
[Route("api")]
[Authorize(Policies.ClientApp)]
public class AchievementController(AchievementDbContext dbContext) : ControllerBase
{

	/// <summary>
	/// 用于获取当前登录的客户端的标识的辅助方法。
	/// </summary>
	/// <returns>登录客户端的 ClientId 信息。</returns>
	private string GetCurrentClientId()
	{
		return User.FindFirstValue(JwtClaimTypes.ClientId)
			?? throw new InvalidOperationException("无法获得用户的客户端标识。");
	}

	/// <summary>
	/// 提供简单的响应以确认服务器状态。
	/// </summary>
	/// <returns>操作结果。</returns>
	[HttpGet("ping")]
	public ActionResult Ping()
	{
		return Ok();
	}


	/// <summary>
	/// 更新应用关联的成就信息。
	/// </summary>
	/// <param name="info">要更新的成就数据相关信息。</param>
	/// <returns>成就系统注册操作的相关反馈信息。</returns>
	[HttpPut("achievement")]
	public async Task<ActionResult<AchievementRegisterResponse>> Put([FromBody] AchievementRegisterInfo info)
	{
		var cancellationToken = HttpContext.RequestAborted;

		var clientId = GetCurrentClientId();

		var currentCategory =
			await (from c in dbContext.Categories
				   where c.AppId == clientId
				   select c)
				.Include(p => p.Items)
				.SingleOrDefaultAsync(cancellationToken);

		if (currentCategory == null)
		{
			return BadRequest("当前应用未在成就系统中注册，请联系管理员。");
		}

		var newItems =
			(from i in info.Items
			 select new AchievementItem
			 {
				 CategoryName = currentCategory.CodeName,
				 CodeName = i.CodeName,
				 DisplayName = i.DisplayName,
				 Description = i.Description,
				 Hint = i.Hint,
				 IconUri = i.IconUri,
				 GrayedIconUri = i.GrayedIconUri,
				 IsDynamic = i.IsDynamic,
				 MaxValue = i.MaxValue,
				 Reward = i.Reward,
				 State = i.State,
			 }).ToArray();

		var config = new BulkConfig();

		// 重新排序
		if (info.Options.ReorderItems)
		{
			newItems.ForEach((item, index) => item.SortOrder = index);
		}
		// 否则，更新时忽略排序顺序
		else
		{
			config.PropertiesToExcludeOnUpdate = [nameof(AchievementItem.SortOrder)];
		}

		// 如果删除未列出项目，则设置分类名为对比条件
		if (info.Options.RemoveAllNonListedItems)
		{
			config.SetSynchronizeFilter<AchievementItem>(i => i.CategoryName == currentCategory.CodeName);
		}

		try
		{
			// 如果要求删除未列出项目，则执行删除，删除同步的标准为当前 categoryName 匹配
			if (info.Options.RemoveAllNonListedItems)
			{
				await dbContext.BulkInsertOrUpdateOrDeleteAsync(newItems, config, cancellationToken: cancellationToken);
			}
			else
			{
				await dbContext.BulkInsertOrUpdateAsync(newItems, config, cancellationToken: cancellationToken);
			}

			await dbContext.BulkSaveChangesAsync(cancellationToken: cancellationToken);
			return new AchievementRegisterResponse();
		}
		catch (DbUpdateException ex)
		{
			return BadRequest(ex.GetBaseMessage());
		}
	}

	/// <summary>
	/// 设置用户的一个或多个成就的状态。
	/// </summary>
	/// <param name="info">用户和成就信息。</param>
	/// <returns>表示异步操作的任务。</returns>
	[HttpPost("user-achievement")]
	public async Task<ActionResult> SetUserAchievements([FromBody] UserAchievementListInfo info)
	{
		var cancellationToken = HttpContext.RequestAborted;

		var clientId = GetCurrentClientId();

		var currentCategory =
			await (from c in dbContext.Categories
				   where c.AppId == clientId
				   select c)
				.SingleOrDefaultAsync(cancellationToken);

		if (currentCategory == null)
		{
			return BadRequest("当前应用未在成就系统中注册，请联系管理员。");
		}

		// 批量生成和更新
		var updatedItems =
			from i in info.Items
			select new AchievementRecord
			{
				CategoryName = currentCategory.CodeName,
				AchievementName = i.CodeName,
				IsCompleted = i.IsCompleted,
				CurrentValue = i.Value,
				Time = DateTimeOffset.Now,
				UserName = info.UserName
			};

		try
		{
			var config = new BulkConfig();

			
			if (!info.ForceOverride)
			{
				// 更新覆盖条件：
				// 原有成就未完成，且无任何进度，或者进度小于当前进度
				config.OnConflictUpdateWhereSql = (oldValue, newValue) => $"({oldValue}.[IsCompleted] = 0) AND ({oldValue}.[CurrentValue] IS NULL OR {oldValue}.[CurrentValue] < {newValue}.[CurrentValue])";
			}

			await dbContext.BulkInsertOrUpdateAsync(updatedItems.ToArray(), bulkConfig: config, cancellationToken: cancellationToken);
			await dbContext.BulkSaveChangesAsync(cancellationToken: cancellationToken);
			return NoContent();
		}
		catch (DbUpdateException ex)
		{
			return BadRequest(ex.GetBaseException());
		}
	}


	/// <summary>
	/// 删除满足条件的成就记录。
	/// </summary>
	/// <param name="userName">如果给定了该参数，则删除条件为仅限该用户的成就记录。</param>
	/// <param name="achievementCodeName">如果给定了该参数，则删除条件为仅限该成就对应的用户记录。</param>
	/// <remarks>
	/// <paramref name="userName"/> 和 <paramref name="achievementCodeName"/> 参数可单独使用，用于指定删除单个用户的所有成就，或单个成就的所有用户记录。如两个参数一起使用，则效果为删除给定用户给定成就的单项纪录。
	/// </remarks>
	/// <returns>表示异步操作的任务。</returns>
	[HttpDelete("user-achievement")]
	public async Task<ActionResult> RemoveUserAchievement([FromQuery] string? userName, [FromQuery] string? achievementCodeName)
	{
		var cancellationToken = HttpContext.RequestAborted;
		var clientId = GetCurrentClientId();

		var items = from i in dbContext.Records
					where i.Achievement.Category.AppId == clientId
					select i;

		if (userName != null)
		{
			items = from i in items
					where i.UserName == userName
					select i;
		}

		if (achievementCodeName != null)
		{
			items = from i in items
					where i.Achievement.CodeName == achievementCodeName
					select i;
		}


		try
		{
			await items.ExecuteDeleteAsync(cancellationToken);
			return Ok();
		}
		catch (DbUpdateException ex)
		{
			return BadRequest(ex.GetBaseMessage());
		}
	}

	/// <summary>
	/// 获取单个用户的所有成就数据。
	/// </summary>
	/// <param name="userName">用户名。</param>
	/// <returns></returns>
	[HttpGet("achievement")]
	public async Task<ActionResult<IEnumerable<RecordInfo>>> GetUserAchievements([FromQuery] string userName)
	{
		var cancellationToken = HttpContext.RequestAborted;
		var clientId = GetCurrentClientId();

		var items =
			from r in dbContext.Records
			where r.Achievement.Category.AppId == clientId && r.UserName == userName
			select new RecordInfo
			{
				UserName = userName,
				AchievementCodeName = r.AchievementName,
				CurrentValue = r.CurrentValue,
				IsCompleted = r.IsCompleted,
				LastUpdateTime = r.Time
			};

		return await items.ToArrayAsync(cancellationToken);
	}

	/// <summary>
	/// 获取当前应用关联的分类信息。
	/// </summary>
	/// <returns></returns>
	[HttpGet("category")]
	public async Task<ActionResult<CategoryInfo?>> GetCategoryInfo()
	{
		var cancellationToken = HttpContext.RequestAborted;

		var clientId = GetCurrentClientId();
		var category =
			await (from c in dbContext.Categories
				   where c.AppId == clientId
				   select c).SingleOrDefaultAsync(cancellationToken);


		return category != null
			? new CategoryInfo
			{
				DisplayName = category.DisplayName,
				DefaultIconUri = category.DefaultIconUri,
				DefaultHideIconUri = category.DefaultHideIconUri,
				AppIconUri = category.AppIconUri,
				UserCount = category.UserCount
			}
			: null;
	}

	/// <summary>
	/// 设置当前应用关联的分类信息。
	/// </summary>
	/// <param name="info"></param>
	/// <returns></returns>
	[HttpPut("category")]
	public async Task<ActionResult> SetCategoryInfo([FromBody] CategoryInfo info)
	{
		var cancellationToken = HttpContext.RequestAborted;

		var clientId = GetCurrentClientId();
		var category =
			await (from c in dbContext.Categories
				   where c.AppId == clientId
				   select c).SingleOrDefaultAsync(cancellationToken);

		if (category == null)
		{
			return BadRequest("当前应用未在成就系统中注册，请联系管理员。");
		}

		category.DisplayName = info.DisplayName;
		category.UserCount = info.UserCount;
		category.DefaultIconUri = info.DefaultIconUri;
		category.DefaultHideIconUri = info.DefaultHideIconUri;
		category.AppIconUri = info.AppIconUri;

		try
		{
			await dbContext.SaveChangesAsync(cancellationToken);
			return NoContent();
		}
		catch (DbUpdateException ex)
		{
			return BadRequest(ex.GetBaseMessage());
		}
	}
}