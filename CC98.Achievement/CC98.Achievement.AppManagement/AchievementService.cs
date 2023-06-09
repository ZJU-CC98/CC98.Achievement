﻿using IdentityModel.Client;

using RestSharp;
using RestSharp.Authenticators.OAuth2;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CC98.Achievement.AppManagement;

/// <summary>
/// 提供成就的相关管理服务。
/// </summary>
[PublicAPI]
public class AchievementService
{
	/// <summary>
	/// 用于进行后台通信的 HTTP 客户端。
	/// </summary>
	private HttpClient HttpClient { get; }

	/// <summary>
	/// 配置选项。
	/// </summary>
	private AchievementServiceOptions Options { get; }

	/// <summary>
	/// 用于包装 RESTful 操作的服务对象。
	/// </summary>
	private RestClient? RestClient { get; set; }



	/// <summary>
	/// 初始化 <see cref="AchievementService"/> 对象的新实例。
	/// </summary>
	/// <param name="httpClient"><see cref="System.Net.Http.HttpClient"/> 服务对象。</param>
	/// <param name="options">服务相关的配置选项。</param>
	/// <exception cref="ArgumentNullException"><paramref name="httpClient"/> 或 <paramref name="options"/> 为 <c>null</c>。</exception>
	public AchievementService(HttpClient httpClient, AchievementServiceOptions options)
	{
		HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		Options = options ?? throw new ArgumentNullException(nameof(options));
	}

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
			await HttpClient.GetDiscoveryDocumentAsync(Options.Authority ?? AchievementServiceDefaults.Authority,
				cancellationToken);

		if (document.IsError)
		{
			throw new InvalidOperationException(document.Error);
		}

		var tokenResponse = await HttpClient.RequestClientCredentialsTokenAsync(new()
		{
			ClientId = Options.ClientId ?? throw new InvalidOperationException("未提供客户端标识。"),
			ClientSecret = Options.ClientSecret ?? throw new InvalidOperationException("未提供客户端机密"),
			Address = document.TokenEndpoint,
			Scope = AchievementScopes.PushAchievement,
		}, cancellationToken);

		if (tokenResponse.IsError)
		{
			throw new InvalidOperationException(tokenResponse.ErrorDescription ?? tokenResponse.Error);
		}


		var options = new RestClientOptions
		{
			BaseUrl = new(Options.ApiBaseUri ?? AchievementServiceDefaults.ApiBaseUri, UriKind.RelativeOrAbsolute),
			Authenticator =
				new OAuth2AuthorizationRequestHeaderAuthenticator(tokenResponse.AccessToken!, tokenResponse.TokenType!),
		};

		RestClient = new(HttpClient, options);
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
		if (info == null)
		{
			throw new ArgumentNullException(nameof(info));
		}

		await EnsureLogOnAsync(cancellationToken);
		return await RestClient!.TryExecuteJsonAsync<AchievementRegisterInfo, AchievementRegisterResponse>(
			Method.Put, "achievement", info, cancellationToken);
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
		if (info == null)
		{
			throw new ArgumentNullException(nameof(info));
		}

		await EnsureLogOnAsync(cancellationToken);
		await RestClient!.TryExecuteJsonAsync(Method.Post, "user-achievements", info, cancellationToken);
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
		if (info == null) throw new ArgumentNullException(nameof(info));
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