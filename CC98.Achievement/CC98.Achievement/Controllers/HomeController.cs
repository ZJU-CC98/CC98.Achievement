using CC98.Achievement.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;


namespace CC98.Achievement.Controllers;

public class HomeController : Controller
{
	public ILogger<HomeController> Logger { get; }

	public HomeController(ILogger<HomeController> logger)
	{
		Logger = logger;
	}

	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}