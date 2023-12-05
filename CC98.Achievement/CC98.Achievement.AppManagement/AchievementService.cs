using IdentityModel.Client;

using RestSharp;
using RestSharp.Authenticators.OAuth2;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CC98.Achievement.AppManagement;

/// <summary>
/// 提供成就的相关管理服务。
/// </summary>
/// <param name="httpClient">用于后台通信的 HTTP 服务。</param>
/// <param name="options">服务选项。</param>
[PublicAPI]
public class AchievementService(HttpClient httpClient, AchievementServiceOptions options)
{
	/// <summary>
	/// 用于包装 RESTful 操作的服务对象。
	/// </summary>
	private RestClient? RestClient { get; set; }

	/// <summary>
	/// 初始化 RESTful 服务。
	/// </summary>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	private async Task InitializeRestClientAsync(CancellationToken cancellationToken = default)
	{
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		if (RestClient != null)
		{
			return;
		}

		await LogOnAsClientAsync(cancellationToken);
	}

	/// <summary>
	/// 确保客户端已经登录。
	/// </summary>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	private async Task EnsureLogOnAsync(CancellationToken cancellationToken = default)
	{
		const int maxTryCount = 1;

		for (var i = 0; i < maxTryCount; i++)
		{
			try
			{
				await InitializeRestClientAsync(cancellationToken);
				await PingAsync(cancellationToken);
			}
			catch (Exception)
			{
				RestClient = null;
			}
		}

		if (RestClient == null)
		{
			throw new InvalidOperationException("无法登录到成就系统。");
		}
	}

	/// <summary>
	/// 尝试使用给定的客户端凭据登录。
	/// </summary>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	/// <exception cref="InvalidOperationException">无法连接到登录服务，或登录信息无效。</exception>
	public async Task LogOnAsClientAsync(CancellationToken cancellationToken = default)
	{
		var document =
			await httpClient.GetDiscoveryDocumentAsync(options.Authority, cancellationToken);

		if (document.IsError)
		{
			throw new InvalidOperationException(document.Error);
		}

		var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new()
		{
			ClientId = options.ClientId ?? throw new InvalidOperationException("未提供客户端标识。"),
			ClientSecret = options.ClientSecret ?? throw new InvalidOperationException("未提供客户端机密"),
			Address = document.TokenEndpoint,
			Scope = AchievementScopes.PushAchievement,
		}, cancellationToken);

		if (tokenResponse.IsError)
		{
			throw new InvalidOperationException(tokenResponse.ErrorDescription ?? tokenResponse.Error);
		}


		var restOptions = new RestClientOptions
		{
			BaseUrl = new(options.ApiBaseUri, UriKind.RelativeOrAbsolute),
			Authenticator =
				new OAuth2AuthorizationRequestHeaderAuthenticator(tokenResponse.AccessToken!, tokenResponse.TokenType!),
		};

		RestClient = new(httpClient, restOptions);
	}

	/// <summary>
	/// 尝试向成就系统注册一系列成就信息。
	/// </summary>
	/// <param name="info">要注册的成就信息和相关选项。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="info"/> 为 <c>null</c>。</exception>
	public async Task<AchievementRegisterResponse> RegisterAchievementsAsync(AchievementRegisterInfo info,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(info);

		await EnsureLogOnAsync(cancellationToken);
		return await RestClient!.TryExecuteJsonAsync<AchievementRegisterInfo, AchievementRegisterResponse>(
			Method.Put, "achievement", info, cancellationToken);
	}

	/// <summary>
	/// 尝试获取给定用户当前的所有成就进度信息。
	/// </summary>
	/// <param name="userName">用户名。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。操作结果包含给定用户当前所有成就及其进度信息。</returns>
	public async Task<IReadOnlyCollection<RecordInfo>> GetUserAchievementsAsync(string userName, CancellationToken cancellationToken = default)
	{
		await EnsureLogOnAsync(cancellationToken);
		return await RestClient!.TryExecuteJsonAsync<IReadOnlyCollection<RecordInfo>>(Method.Get,
			$"achievement?userName={Uri.EscapeDataString(userName)}", cancellationToken);
	}

	/// <summary>
	/// 尝试更新单个单个成就项信息。
	/// </summary>
	/// <param name="info">成就项信息。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="info"/> 为 <c>null</c>。</exception>
	/// <exception cref="InvalidOperationException">更新成就信息失败。</exception>
	public async Task SetUserAchievementsAsync(UserAchievementListInfo info, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(info);

		await EnsureLogOnAsync(cancellationToken);
		await RestClient!.TryExecuteJsonAsync(Method.Post, "user-achievement", info, cancellationToken);
	}

	/// <summary>
	/// 尝试获得当前应用对应的分类数据。
	/// </summary>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。任务结果为对应的分类数据。</returns>
	public async Task<CategoryInfo> GetCategoryInfoAsync(CancellationToken cancellationToken = default)
	{
		await EnsureLogOnAsync(cancellationToken);
		return await RestClient!.TryExecuteJsonAsync<CategoryInfo>(Method.Get, "category", cancellationToken);
	}

	/// <summary>
	/// 尝试更新当前应用对应的分类数据。
	/// </summary>
	/// <param name="info">新的分类数据信息。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	public async Task UpdateCategoryInfoAsync(CategoryInfo info, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(info);

		await EnsureLogOnAsync(cancellationToken);
		await RestClient!.TryExecuteJsonAsync(Method.Put, "category", info, cancellationToken);
	}

	/// <summary>
	/// 执行服务器响应测试。
	/// </summary>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	public async Task PingAsync(CancellationToken cancellationToken = default)
	{
		// 注意 PING 会用于检测登录，所以不能调用 EnsureLogOn
		await InitializeRestClientAsync(cancellationToken);
		await RestClient!.TryExecuteJsonAsync(Method.Get, "ping", cancellationToken);
	}
}