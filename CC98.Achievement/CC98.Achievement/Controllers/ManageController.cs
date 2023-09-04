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
/// 提供系统管理功能。
/// </summary>
[Authorize(Policies.Admin)]
public class ManageController : Controller
{
	/// <inheritdoc />
	public ManageController(IConfiguration configuration, IOperationMessageAccessor messageAccessor, IDynamicHtmlLocalizer<SharedResources> sharedResourceLocalizer, IDynamicHtmlLocalizer<ManageController> localizer, AppSettingService<SystemSetting> appSettingService, AchievementBackService backService)
	{
		Configuration = configuration;
		MessageAccessor = messageAccessor;
		SharedResourceLocalizer = sharedResourceLocalizer;
		Localizer = localizer;
		AppSettingService = appSettingService;
		BackService = backService;
	}

	/// <summary>
	/// 系统设置服务。
	/// </summary>
	private AppSettingService<SystemSetting> AppSettingService { get; }

	/// <summary>
	/// 应用程序配置。
	/// </summary>
	private IConfiguration Configuration { get; }

	/// <summary>
	/// 消息服务。
	/// </summary>
	private IOperationMessageAccessor MessageAccessor { get; }

	/// <summary>
	/// 公用字符串资源。
	/// </summary>
	private IDynamicHtmlLocalizer<SharedResources> SharedResourceLocalizer { get; }

	/// <summary>
	/// 字符串资源。
	/// </summary>
	private IDynamicHtmlLocalizer<ManageController> Localizer { get; }

	/// <summary>
	/// 后台 API 服务。
	/// </summary>
	private AchievementBackService BackService { get; }

	/// <summary>
	/// 获取系统预定义的成就项目。
	/// </summary>
	private static AchievementItemRegisterInfo[] AchievementList { get; } = LoadPredefinedItems();

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
	private static AchievementItemRegisterInfo[] LoadPredefinedItems()
	{
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
		options.Converters.Add(new JsonStringEnumConverter());

		return JsonSerializer.Deserialize<AchievementItemRegisterInfo[]>(Properties.Resources.AchievementList, options)!;

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
			var result = await BackService.InnerService.RegisterAchievementsAsync(new()
			{
				Items = AchievementList,
				Options = new()
				{
					ReorderItems = true
				}
			}, cancellationToken);

			Utility.AddMessage(MessageAccessor, OperationMessageLevel.Success,
				SharedResourceLocalizer.Html.OperationSucceeded, Localizer.Html.RegisterSucceeded(
					result.NewItemCount, result.UpdatedItemCount,
					result.DeletedItemCount));
		}
		catch (Exception ex)
		{
			MessageAccessor.Add(OperationMessageLevel.Error, "操作失败", ex.GetBaseMessage());
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
		return View(AppSettingService.Current);
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
			AppSettingService.Current = model;
			Utility.AddMessage(MessageAccessor, OperationMessageLevel.Success, SharedResourceLocalizer.Html.OperationSucceeded, Localizer.Html.SystemSettingUpdated);

			RedirectToAction("SystemSetting", "Manage");
		}

		return View(model);
	}
}