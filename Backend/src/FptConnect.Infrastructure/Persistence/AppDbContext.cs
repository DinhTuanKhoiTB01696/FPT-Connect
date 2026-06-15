using FptConnect.Application.Common;
using FptConnect.Domain.Common;
using FptConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerStatusHistory> CustomerStatusHistory => Set<CustomerStatusHistory>();

    // IAM — Sprint 1
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<MfaMethod> MfaMethods => Set<MfaMethod>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.ToTable("Users", "iam");
            e.HasIndex(x => x.EmailNormalized).IsUnique();
            e.HasIndex(x => x.EmployeeCode).IsUnique();
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.FullName).HasMaxLength(200);
            e.Property(x => x.EmailNormalized).HasMaxLength(256);
        });

        b.Entity<Role>(e =>
        {
            e.ToTable("Roles", "iam");
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.RowVersion).IsRowVersion();
        });

        b.Entity<UserRole>(e =>
        {
            e.ToTable("UserRoles", "iam");
            e.HasKey(x => new { x.UserId, x.RoleId });
            e.HasOne(x => x.User).WithMany(u => u.Roles).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
        });

        b.Entity<Session>(e =>
        {
            e.ToTable("Sessions", "iam");
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.RefreshTokenHash).HasMaxLength(32);
            e.Property(x => x.RevokedReason).HasMaxLength(200);
            e.HasIndex(x => x.RefreshTokenHash).IsUnique();
            e.HasIndex(x => new { x.UserId, x.RevokedAtUtc });
            e.HasIndex(x => x.TokenFamilyId);
        });

        b.Entity<Device>(e =>
        {
            e.ToTable("Devices", "iam");
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.Name).HasMaxLength(120);
            e.Property(x => x.Platform).HasMaxLength(60);
            e.Property(x => x.RiskStatus).HasMaxLength(20);
            e.HasIndex(x => new { x.UserId, x.DeviceKeyHash }).IsUnique();
            e.HasIndex(x => new { x.UserId, x.LastSeenAtUtc });
        });

        b.Entity<MfaMethod>(e =>
        {
            e.ToTable("MfaMethods", "iam");
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.Type).HasMaxLength(20);
            e.HasIndex(x => new { x.UserId, x.Type, x.IsVerified });
        });

        b.Entity<AuditLog>(e =>
        {
            e.ToTable("AuditLogs", "audit");
            e.Property(x => x.Action).HasMaxLength(80);
            e.Property(x => x.ResourceType).HasMaxLength(80);
            e.Property(x => x.ResourceId).HasMaxLength(80);
            e.Property(x => x.EntryHash).HasMaxLength(32);
            e.Property(x => x.PrevHash).HasMaxLength(32);
            e.HasIndex(x => new { x.ResourceType, x.ResourceId, x.OccurredAtUtc });
            e.HasIndex(x => new { x.ActorUserId, x.OccurredAtUtc });
        });

        b.Entity<Customer>(e =>
        {
            e.ToTable("Customers", "crm");
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.FullName).HasMaxLength(200);
            e.Property(x => x.Latitude).HasColumnType("decimal(9,6)");
            e.Property(x => x.Longitude).HasColumnType("decimal(9,6)");
            e.HasIndex(x => new { x.OwnerUserId, x.StatusCode });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        b.Entity<CustomerStatusHistory>(e =>
        {
            e.ToTable("CustomerStatusHistory", "crm");
            e.HasOne(x => x.Customer).WithMany(c => c.StatusHistory).HasForeignKey(x => x.CustomerId);
            e.HasIndex(x => new { x.CustomerId, x.ChangedAtUtc });
        });

        base.OnModelCreating(b);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}
