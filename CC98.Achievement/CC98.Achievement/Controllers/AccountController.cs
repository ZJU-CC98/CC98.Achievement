using CC98.Authentication.OpenIdConnect;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sakura.AspNetCore.Authentication;

namespace CC98.Achievement.Controllers;

/// <summary>
/// 提供登录注销相关的功能。
/// </summary>
/// <param name="externalSignInManager">外部登录服务。</param>
public class AccountController(ExternalSignInManager externalSignInManager) : Controller
{
	/// <summary>
	/// 执行登录操作。
	/// </summary>
	/// <param name="returnUrl">登录后要跳转的地址。</param>
	/// <returns>表示操作结果的响应。</returns>
	[AllowAnonymous]
	public IActionResult LogOn(string? returnUrl)
	{
		var authProperties = new AuthenticationProperties
		{
			// 将重定向地址替换为回调地址
			RedirectUri = Url.Action("LogOnCallback", "Account", new { returnUrl })
		};

		return Challenge(authProperties, CC98Defaults.AuthenticationScheme);
	}

	/// <summary>
	/// 执行登录回调操作。
	/// </summary>
	/// <param name="returnUrl">登录后要返回的地址。</param>
	/// <returns>表示异步操作的任务。操作结果包含网页响应。</returns>
	/// <exception cref="NotImplementedException"></exception>
	[AllowAnonymous]
	public async Task<IActionResult> LogOnCallback(string? returnUrl)
	{
		var principal = await externalSignInManager.SignInFromExternalCookieAsync();

		if (principal?.Identity == null)
		{
			// TODO: 登录失败处理
			throw new NotImplementedException();
		}

		// 防止跳转到外部地址
		if (!Url.IsLocalUrl(returnUrl))
		{
			returnUrl = Url.Action("Index", "Home");
		}

		return Redirect(returnUrl ?? string.Empty);
	}

	/// <summary>
	/// 执行注销操作。
	/// </summary>
	/// <returns>表示异步操作的任务。操作结果包含网页响应。</returns>
	[Authorize]
	public async Task<IActionResult> LogOff()
	{
		await externalSignInManager.SignOutAsync();
		return RedirectToAction("Index", "Home");
	}
}