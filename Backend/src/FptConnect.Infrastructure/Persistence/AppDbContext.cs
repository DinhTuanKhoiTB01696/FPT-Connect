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

        b.Entity<Customer>(e =>
        {
            e.ToTable("Customers", "crm");
            e.Property(x => x.RowVersion).IsRowVersion();
            e.Property(x => x.FullName).HasMaxLength(200);
            e.Property(x => x.Latitude).HasColumnType("decimal(9,6)");
            e.Property(x => x.Longitude).HasColumnType("decimal(9,6)");
            e.HasIndex(x => new { x.OwnerUserId, x.StatusCode });
            // Global query filter soft-delete (Bible 6.1)
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
