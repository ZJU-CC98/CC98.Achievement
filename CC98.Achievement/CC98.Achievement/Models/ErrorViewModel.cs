namespace CC98.Achievement.Models;

/// <summary>
/// 发生错误时的相关信息。
/// </summary>
public class ErrorViewModel
{
	/// <summary>
	/// 获取或设置本次请求的后台唯一标识。
	/// </summary>
	public string? RequestId { get; set; }

	/// <summary>
	/// 获取一个值，指示错误是否具有有效的 <see cref="RequestId"/> 信息。
	/// </summary>
	public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}