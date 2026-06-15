using FptConnect.Application.Common;
using FptConnect.Infrastructure.Persistence;
using FptConnect.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FptConnect.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("Default")
            ?? @"Server=(localdb)\MSSQLLocalDB;Database=FptConnectDB;Trusted_Connection=True;TrustServerCertificate=True";

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(conn, sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<ITotpService, TotpService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuditWriter, AuditWriter>();
        return services;
    }
}
