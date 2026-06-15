using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FptConnect.Infrastructure.Persistence;

/// <summary>
/// DbContext gốc. Chưa khai báo DbSet — entity & cấu hình sẽ thêm theo từng bounded context/sprint
/// (áp dụng qua IEntityTypeConfiguration trong cùng assembly).
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
