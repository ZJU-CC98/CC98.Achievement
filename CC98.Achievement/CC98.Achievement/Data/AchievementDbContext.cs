using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace CC98.Achievement.Data;

/// <summary>
/// 表示成就系统数据库上下文对象。
/// </summary>
/// <param name="options">数据库上下文对象。</param>
[UsedImplicitly(ImplicitUseKindFlags.Assign | ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.Members)]
public class AchievementDbContext(DbContextOptions<AchievementDbContext> options)
	: DbContext(options)
{

	/// <summary>
	/// 获取数据库所有成就项的集合。
	/// </summary>
	public virtual DbSet<AchievementItem> Items { get; set; } = null!;

	/// <summary>
	/// 获取数据库所有成就记录的集合。
	/// </summary>
	public virtual DbSet<AchievementRecord> Records { get; set; } = null!;

	/// <summary>
	/// 获取数据库中所有成就颁发应用的集合。
	/// </summary>

	public virtual DbSet<AchievementCategory> Categories { get; set; } = null!;

	/// <summary>
	/// 获取数据库中所有用户的集合。
	/// </summary>
	public virtual DbSet<User> Users { get; set; } = null!;

	/// <inheritdoc />
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>().HasAlternateKey(i => i.Name);
		base.OnModelCreating(modelBuilder);
	}
}