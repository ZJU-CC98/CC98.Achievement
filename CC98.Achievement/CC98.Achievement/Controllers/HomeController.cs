using CC98.Achievement.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;


namespace CC98.Achievement.Controllers;

/// <summary>
/// 提供网页基本功能。
/// </summary>
public class HomeController : Controller
{
	/// <summary>
	/// 日志服务对象。
	/// </summary>
	private ILogger<HomeController> Logger { get; }

	/// <summary>
	/// 初始化 <see cref="HomeController"/> 对象的新实例。
	/// </summary>
	/// <param name="logger"><see cref="ILogger{HomeController}"/> 服务对象。</param>
	public HomeController(ILogger<HomeController> logger)
	{
		Logger = logger;
	}

	/// <summary>
	/// 显示首页。
	/// </summary>
	/// <returns>操作结果。</returns>
	public IActionResult Index()
	{
		return RedirectToAction("Index", "Achievement");
	}

	/// <summary>
	/// 显示隐私页。
	/// </summary>
	/// <returns>操作结果。</returns>
	public IActionResult Privacy()
	{
		return View();
	}

	/// <summary>
	/// 显示错误页面。
	/// </summary>
	/// <returns>操作结果。</returns>
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}