namespace CC98.Achievement.Models;

/// <summary>
/// ��������ʱ�������Ϣ��
/// </summary>
public class ErrorViewModel
{
	/// <summary>
	/// ��ȡ�����ñ�������ĺ�̨Ψһ��ʶ��
	/// </summary>
	public string? RequestId { get; set; }

	/// <summary>
	/// ��ȡһ��ֵ��ָʾ�����Ƿ������Ч�� <see cref="RequestId"/> ��Ϣ��
	/// </summary>
	public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}