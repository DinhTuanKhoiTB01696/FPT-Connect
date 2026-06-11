using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FptConnect.Infrastructure.Persistence;

/// <summary>
/// Cho phép `dotnet ef migrations` chạy mà không cần khởi động API.
/// Đọc connection từ biến môi trường ConnectionStrings__Default (run.bat set), fallback LocalDB.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? @"Server=(localdb)\MSSQLLocalDB;Database=FptConnectDB;Trusted_Connection=True;TrustServerCertificate=True";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(conn, sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .Options;
        return new AppDbContext(options);
    }
}
