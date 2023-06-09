using CC98.Achievement.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sakura.AspNetCore;
using Sakura.AspNetCore.Localization;

using System.Text.Json;
using System.Text.Json.Serialization;

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
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns></returns>
	private async Task<AchievementItemRegisterInfo[]> LoadPredefinedItemsAsync(CancellationToken cancellationToken = default)
	{
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
		options.Converters.Add(new JsonStringEnumConverter());

		await using var file = System.IO.File.OpenRead("achievements.json");
		var result =
			await JsonSerializer.DeserializeAsync<AchievementItemRegisterInfo[]>(file, options, cancellationToken);

		return result!;
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
				Items = await LoadPredefinedItemsAsync(cancellationToken),
				Options = new(),
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
	/// <returns></returns>
	[HttpGet]
	public IActionResult SystemSetting()
	{
		return View(AppSettingService.Current);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult SystemSetting(SystemSetting model)
	{
		if (ModelState.IsValid)
		{
			AppSettingService.Current = model;
			Utility.AddMessage(MessageAccessor, OperationMessageLevel.Success, SharedResourceLocalizer.Html.OperationSucceeded, Localizer.Html.SystemSettingUpdated);

			RedirectToAction("SystemSetting", "Manage");
		}

		return View(model);
	}
}