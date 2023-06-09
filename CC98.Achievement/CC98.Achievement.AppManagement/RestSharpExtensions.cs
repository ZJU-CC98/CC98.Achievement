using RestSharp;

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CC98.Achievement.AppManagement;

public static class RestSharpExtensions
{

	/// <summary>
	/// 尝试以 JSON 格式发送数据执行 RESTful 请求并获得响应结果。。
	/// </summary>
	/// <typeparam name="TRequest">发送数据的格式。</typeparam>
	/// <typeparam name="TResponse">发送数据的格式。</typeparam>
	/// <param name="client"><see cref="RestClient"/> 服务对象。</param>
	/// <param name="method">执行的方法。</param>
	/// <param name="resource">执行的地址。</param>
	/// <param name="request">执行提供的数据对象。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。任务结果包含响应数据。</returns>
	/// <exception cref="InvalidOperationException">服务器返回了异常响应。</exception>
	public static async Task<TResponse> TryExecuteJsonAsync<TRequest, TResponse>(this RestClient client, Method method, string resource,
		TRequest request, CancellationToken cancellationToken = default)
		where TRequest : class
		where TResponse : class
	{
		var realRequest = new RestRequest(resource, method);
		realRequest.AddJsonBody(request);

		var response = await client.ExecuteAsync<TResponse>(realRequest, cancellationToken);

		// 响应失败，则直接引发异常
		if (!response.IsSuccessful)
		{
			var value = JsonSerializer.Deserialize<string>(response.Content ?? string.Empty);
			throw new InvalidOperationException(value);
		}

		// 响应成功返回数据
		return response.Data ?? throw new InvalidOperationException("服务器未正确反馈数据，请联系管理员");

	}

	/// <summary>
	/// 尝试以 JSON 格式发送数据执行 RESTful 请求。
	/// </summary>
	/// <typeparam name="TRequest">发送数据的格式。</typeparam>
	/// <param name="client"><see cref="RestClient"/> 服务对象。</param>
	/// <param name="method">执行的方法。</param>
	/// <param name="resource">执行的地址。</param>
	/// <param name="request">执行提供的数据对象。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	/// <exception cref="InvalidOperationException">服务器返回了异常响应。</exception>
	public static async Task TryExecuteJsonAsync<TRequest>(this RestClient client, Method method, string resource,
		TRequest request, CancellationToken cancellationToken = default)
		where TRequest : class
	{
		var realRequest = new RestRequest(resource, method);
		realRequest.AddJsonBody(request);

		var response = await client.ExecuteAsync(realRequest, cancellationToken);

		// 响应失败，则直接引发异常
		if (!response.IsSuccessful)
		{
			var value = JsonSerializer.Deserialize<string>(response.Content ?? string.Empty);
			throw new InvalidOperationException(value);
		}
	}

	/// <summary>
	/// 执行 RESTful 请求。
	/// </summary>
	/// <param name="client"><see cref="RestClient"/> 服务对象。</param>
	/// <param name="method">执行的方法。</param>
	/// <param name="resource">执行的地址。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。</returns>
	/// <exception cref="InvalidOperationException">服务器返回了异常响应。</exception>
	public static async Task TryExecuteJsonAsync(this RestClient client, Method method, string resource,
		CancellationToken cancellationToken = default)
	{
		var realRequest = new RestRequest(resource, method);
		var response = await client.ExecuteAsync(realRequest, cancellationToken);

		// 响应失败，则直接引发异常
		if (!response.IsSuccessful)
		{
			var value = JsonSerializer.Deserialize<string>(response.Content ?? string.Empty);
			throw new InvalidOperationException(value);
		}
	}

	/// <summary>
	/// 执行 RESTful 请求并以 JSON 格式获得响应数据。
	/// </summary>
	/// <typeparam name="TResponse">响应数据的格式。</typeparam>
	/// <param name="client"><see cref="RestClient"/> 服务对象。</param>
	/// <param name="method">执行的方法。</param>
	/// <param name="resource">执行的地址。</param>
	/// <param name="cancellationToken">用于取消操作的令牌。</param>
	/// <returns>表示异步操作的任务。任务结果包含响应数据。</returns>
	/// <exception cref="InvalidOperationException">服务器返回了异常响应。</exception>
	public static async Task<TResponse> TryExecuteJsonAsync<TResponse>(this RestClient client, Method method, string resource,
		CancellationToken cancellationToken = default)
		where TResponse : class
	{
		var realRequest = new RestRequest(resource, method);
		var response = await client.ExecuteAsync<TResponse>(realRequest, cancellationToken);

		// 响应失败，则直接引发异常
		if (!response.IsSuccessful || response.Data == null)
		{
			var value = JsonSerializer.Deserialize<string>(response.Content ?? string.Empty);
			throw new InvalidOperationException(value);
		}

		return response.Data;
	}
}