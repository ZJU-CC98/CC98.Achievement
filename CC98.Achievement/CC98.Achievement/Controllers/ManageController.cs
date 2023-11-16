using System.Collections.ObjectModel;
using CC98.Achievement.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sakura.AspNetCore;
using Sakura.AspNetCore.Localization;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CC98.Achievement.Controllers;

/// <summary>
/// 提供系统管理功能
/// </summary>
/// <param name="messageAccessor">消息访问服务。</param>
/// <param name="sharedResourceLocalizer">共享的本地化资源。</param>
/// <param name="localizer">本地化资源。</param>
/// <param name="appSettingService">应用程序设置。</param>
/// <param name="backService">后端服务。</param>
[Authorize(Policies.Admin)]
public class ManageController(IOperationMessageAccessor messageAccessor, IDynamicHtmlLocalizer<SharedResources> sharedResourceLocalizer, IDynamicHtmlLocalizer<ManageController> localizer, AppSettingService<SystemSetting> appSettingService, AchievementBackService backService)
	: Controller
{

	/// <summary>
	/// JSON 序列化的默认配置。
	/// </summary>
	private static JsonSerializerOptions JsonSerializerOptions { get; } = GenerateJsonSerializerOptions();

	/// <summary>
	/// 生成 JSON 序列化的默认配置。
	/// </summary>
	/// <returns></returns>
	private static JsonSerializerOptions GenerateJsonSerializerOptions()
	{
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
		options.Converters.Add(new JsonStringEnumConverter());
		return options;
	}

	/// <summary>
	/// 获取系统预定义的成就项目。
	/// </summary>
	private static ReadOnlyCollection<AchievementItemRegisterInfo> AchievementList { get; } = LoadPredefinedItems();

	/// <summary>
	/// 显示管理界面主页。
	/// </summary>
	/// <returns>操作结果。</returns>
	[HttpGet]
	public IActionResult Index()
	{
		return View();
	}


	/// <summary>
	/// 从系统文件中加载预制的成就数据。
	/// </summary>
	/// <returns></returns>
	private static ReadOnlyCollection<AchievementItemRegisterInfo> LoadPredefinedItems()
	{
		return JsonSerializer.Deserialize<AchievementItemRegisterInfo[]>(Properties.Resources.AchievementList, JsonSerializerOptions)!.AsReadOnly();
	}

	/// <summary>
	/// 向系统注册自身的相关成就项目。
	/// </summary>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。任务结果为操作结果。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RegisterItems(CancellationToken cancellationToken)
	{
		try
		{
			var result = await backService.InnerService.RegisterAchievementsAsync(new()
			{
				Items = AchievementList,
				Options = new()
				{
					ReorderItems = true
				}
			}, cancellationToken);

			Utility.AddMessage(messageAccessor, OperationMessageLevel.Success,
				sharedResourceLocalizer.Html.OperationSucceeded, localizer.Html.RegisterSucceeded(
					result.NewItemCount, result.UpdatedItemCount,
					result.DeletedItemCount));
		}
		catch (Exception ex)
		{
			messageAccessor.Add(OperationMessageLevel.Error, "操作失败", ex.GetBaseMessage());
		}

		return RedirectToAction("Index", "Manage");
	}

	/// <summary>
	/// 显示系统设置页面。
	/// </summary>
	/// <returns>操作结果。</returns>
	[HttpGet]
	public IActionResult SystemSetting()
	{
		return View(appSettingService.Current);
	}

	/// <summary>
	/// 执行系统更改操作。
	/// </summary>
	/// <param name="model">数据模型。</param>
	/// <returns>操作结果。</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult SystemSetting(SystemSetting model)
	{
		// 移除对模板无意义的属性要求
		ModelState.Remove<SystemSetting>(i => i.HiddenItemTemplate.CodeName);
		ModelState.Remove<SystemSetting>(i => i.HiddenItemTemplate.Category);
		ModelState.Remove<SystemSetting>(i => i.HiddenItemTemplate.CategoryName);


		// 强制设置为普通显示模式
		model.HiddenItemTemplate.State = AchievementState.Normal;

		if (ModelState.IsValid)
		{
			appSettingService.Current = model;
			Utility.AddMessage(messageAccessor, OperationMessageLevel.Success, sharedResourceLocalizer.Html.OperationSucceeded, localizer.Html.SystemSettingUpdated);

			RedirectToAction("SystemSetting", "Manage");
		}

		return View(model);
	}
}