using System.ComponentModel.DataAnnotations;

namespace CC98.Achievement.Models;

/// <summary>
/// ����ɾ͵����״̬��
/// </summary>
public enum AchievementCompleteState
{
	/// <summary>
	/// δ��ɡ�
	/// </summary>
	[Display(ShortName = "δ���")]
	None,
	/// <summary>
	/// ����δ��ȫ��ɡ�
	/// </summary>
	[Display(ShortName = "�����")]
	Progress,
	/// <summary>
	/// ����ɡ�
	/// </summary>
	[Display(ShortName = "�ѻ��")]
	Completed
}