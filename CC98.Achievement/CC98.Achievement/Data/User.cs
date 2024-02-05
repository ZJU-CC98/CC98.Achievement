using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.Achievement.Data;

/// <summary>
/// 表示一个 CC98 用户。
/// </summary>
[Table("Users")]
public class User
{
	/// <summary>
	/// 用户的标识。
	/// </summary>
	[Key]
	public int Id { get; set; }

	/// <summary>
	/// 用户名。
	/// </summary>
	[Required]
	[StringLength(50)]
	public required string Name { get; set; }

	/// <summary>
	/// 用户的头像 URL 地址。
	/// </summary>
	[Required]
	[Url]
	public required string PortraitUrl { get; set; }

	/// <summary>
	/// 用户的性别。
	/// </summary>

	[Column("Sex")]
	[Required]
	public Gender Gender { get; set; }
}