using CC98.Achievement.Data;

using IdentityModel;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace CC98.Achievement.Areas.Api;

/// <summary>
/// 为第三方 API 提供 RESTful 调用接口。
/// </summary>
[Area("Api")]
[Route("api")]
[Authorize(Policies.ClientApp)]
public class AchievementController : ControllerBase
{
	/// <summary>
	/// 初始化 <see cref="AchievementController"/> 对象的新实例。
	/// </summary>
	/// <param name="dbContext"><see cref="AchievementDbContext"/> 服务对象。</param>
	public AchievementController(AchievementDbContext dbContext)
	{
		DbContext = dbContext;
	}

	/// <summary>
	/// 数据库上下文对象。
	/// </summary>
	private AchievementDbContext DbContext { get; set; }

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
			await (from c in DbContext.Categories
				   where c.AppId == clientId
				   select c)
				.Include(p => p.Items)
				.SingleOrDefaultAsync(cancellationToken);

		if (currentCategory == null)
		{
			return BadRequest("当前应用未在成就系统中注册，请联系管理员。");
		}

		// 记录尚未处理过的项目
		var unhandledItems = new HashSet<AchievementItem>(currentCategory.Items);
		var result = new AchievementRegisterResponse();
		var index = 0;

		foreach (var item in info.Items)
		{
			var databaseItem = currentCategory.Items.FirstOrDefault(i => i.CodeName == item.CodeName);

			if (databaseItem == null)
			{
				databaseItem = new()
				{
					CodeName = item.CodeName,
					Category = currentCategory
				};

				DbContext.Items.Add(databaseItem);

				result.NewItemCount++;
			}
			else
			{
				unhandledItems.Remove(databaseItem);
				result.UpdatedItemCount++;
			}


			databaseItem.DisplayName = item.DisplayName;
			databaseItem.Description = item.Description;
			databaseItem.Hint = item.Hint;
			databaseItem.Reward = item.Reward;
			databaseItem.IconUri = item.IconUri;
			databaseItem.MaxValue = item.MaxValue;
			databaseItem.State = item.State;

			if (info.Options.ReorderItems)
			{
				databaseItem.SortOrder = index;
			}

			index++;
		}

		// 删除多余项目
		if (info.Options.ReorderItems)
		{
			result.DeletedItemCount = unhandledItems.Count;
			DbContext.Items.RemoveRange(unhandledItems);
		}

		try
		{
			await DbContext.SaveChangesAsync(cancellationToken);
			return result;
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
	[HttpPost("user-achievements")]
	public async Task<ActionResult> SetUserAchievement([FromBody] UserAchievementListInfo info)
	{
		var cancellationToken = HttpContext.RequestAborted;

		var clientId = GetCurrentClientId();

		var currentCategory =
			await (from c in DbContext.Categories
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

		DbContext.Records.UpdateRange(updatedItems);

		try
		{
			await DbContext.SaveChangesAsync(cancellationToken);
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

		var items = from i in DbContext.Records
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

		DbContext.RemoveRange(items);

		try
		{
			await DbContext.SaveChangesAsync(cancellationToken);
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
			from r in DbContext.Records
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
			await (from c in DbContext.Categories
				   where c.AppId == clientId
				   select c).SingleOrDefaultAsync(cancellationToken);


		return category != null
			? new CategoryInfo
			{
				DisplayName = category.DisplayName,
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
			await (from c in DbContext.Categories
				   where c.AppId == clientId
				   select c).SingleOrDefaultAsync(cancellationToken);

		if (category == null)
		{
			return BadRequest("当前应用未在成就系统中注册，请联系管理员。");
		}

		category.DisplayName = info.DisplayName;
		category.UserCount = info.UserCount;

		try
		{
			await DbContext.SaveChangesAsync(cancellationToken);
			return NoContent();
		}
		catch (DbUpdateException ex)
		{
			return BadRequest(ex.GetBaseMessage());
		}
	}
}