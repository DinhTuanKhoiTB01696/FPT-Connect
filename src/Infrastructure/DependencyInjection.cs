using FptConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FptConnect.Infrastructure;

/// <summary>Đăng ký tầng Infrastructure (EF Core, providers). Chưa có repository/service cụ thể.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("Default")
            ?? @"Server=(localdb)\MSSQLLocalDB;Database=FptConnectDB;Trusted_Connection=True;TrustServerCertificate=True";

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(conn, sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        return services;
    }
}
