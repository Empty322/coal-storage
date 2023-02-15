using DevExpress.ExpressApp.EFCore.Updating;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.EFCore.DesignTime;
using DevExpress.Persistent.BaseImpl.EFCore.AuditTrail;

namespace Storage.Module.BusinessObjects;

// This code allows our Model Editor to get relevant EF Core metadata at design time.
// For details, please refer to https://supportcenter.devexpress.com/ticket/details/t933891.
public class StorageContextInitializer : DbContextTypesInfoInitializerBase {
	protected override DbContext CreateDbContext() {
		var optionsBuilder = new DbContextOptionsBuilder<StorageEFCoreDbContext>()
            .UseSqlServer(";")
            .UseChangeTrackingProxies()
            .UseObjectSpaceLinkProxies();
        return new StorageEFCoreDbContext(optionsBuilder.Options);
	}
}
//This factory creates DbContext for design-time services. For example, it is required for database migration.
public class StorageDesignTimeDbContextFactory : IDesignTimeDbContextFactory<StorageEFCoreDbContext> {
	public StorageEFCoreDbContext CreateDbContext(string[] args) {
		var optionsBuilder = new DbContextOptionsBuilder<StorageEFCoreDbContext>();
		optionsBuilder.UseSqlServer("Server=localhost;Initial Catalog=Storage;User ID=sa;Password=RootToor1");
        optionsBuilder.UseChangeTrackingProxies();
        optionsBuilder.UseObjectSpaceLinkProxies();
		return new StorageEFCoreDbContext(optionsBuilder.Options);
	}
}

[TypesInfoInitializer(typeof(StorageContextInitializer))]
public class StorageEFCoreDbContext : DbContext {
	public StorageEFCoreDbContext(DbContextOptions<StorageEFCoreDbContext> options) : base(options) {
	}
	public DbSet<Storage> Storages { get; set; }
	public DbSet<Picket> Pickets { get; set; }
	public DbSet<Area> Areas { get; set; }

	public DbSet<PermissionPolicyUser> PermissionPolicyUsers { get; set; }
	public DbSet<PermissionPolicyRole> PermissionPolicyRoles { get; set; }

	public DbSet<AuditDataItemPersistent> AuditData { get; set; }
	public DbSet<AuditEFCoreWeakReference> AuditEFCoreWeakReferences { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues);

		modelBuilder.Entity<AuditEFCoreWeakReference>()
			.HasMany(p => p.AuditItems)
			.WithOne(p => p.AuditedObject);
		modelBuilder.Entity<AuditEFCoreWeakReference>()
			.HasMany(p => p.OldItems)
			.WithOne(p => p.OldObject);
		modelBuilder.Entity<AuditEFCoreWeakReference>()
			.HasMany(p => p.NewItems)
			.WithOne(p => p.NewObject);
		modelBuilder.Entity<AuditEFCoreWeakReference>()
			.HasMany(p => p.UserItems)
			.WithOne(p => p.UserObject);
	}
}

public class StorageAuditingDbContext : DbContext
{
	public StorageAuditingDbContext(DbContextOptions<StorageAuditingDbContext> options) : base(options)
	{
	}
	public DbSet<AuditDataItemPersistent> AuditData { get; set; }
	public DbSet<AuditEFCoreWeakReference> AuditEFCoreWeakReferences { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues);
		modelBuilder.Entity<AuditEFCoreWeakReference>()
			.HasMany(p => p.AuditItems)
			.WithOne(p => p.AuditedObject);
		modelBuilder.Entity<AuditEFCoreWeakReference>()
			.HasMany(p => p.OldItems)
			.WithOne(p => p.OldObject);
		modelBuilder.Entity<AuditEFCoreWeakReference>()
			.HasMany(p => p.NewItems)
			.WithOne(p => p.NewObject);
		modelBuilder.Entity<AuditEFCoreWeakReference>()
			.HasMany(p => p.UserItems)
			.WithOne(p => p.UserObject);
	}
}

