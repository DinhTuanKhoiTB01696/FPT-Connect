using System.Text;
using System.Threading.RateLimiting;
using FptConnect.Api.Security;
using FptConnect.Application;
using FptConnect.Application.Common;
using FptConnect.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;

using FptConnect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddDataProtection();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string CorsPolicy = "frontend";
builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p => p
    .WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? new[] { "http://localhost:5173" })
    .AllowAnyHeader().AllowAnyMethod()));

// Rate limit — login/MFA: theo IP, fixed window (TC-010, Bible 3.5)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// ── Auto-seed: đảm bảo admin user luôn tồn tại ──────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Seed");

    try
    {
        // Schema tự động (dựa hoàn toàn vào ConnectionStrings:Default trong appsettings — không cần gõ tay).
        //  - Production: dùng migrations nếu có.
        //  - Development: dựng schema từ model; nếu DB cũ thiếu bảng (schema drift) thì TỰ tạo lại,
        //    không cần drop tay / không cần sqlcmd.
        // Công tắc reset: đặt FptConnect:RecreateDatabase=true (env FptConnect__RecreateDatabase=true)
        // để xoá & tạo lại DB tự động — KHÔNG cần sqlcmd/drop tay. run.bat dùng cờ này khi chọn "Y".
        var recreate = app.Configuration.GetValue<bool>("FptConnect:RecreateDatabase");
        if (recreate)
        {
            logger.LogWarning("RecreateDatabase=true -> xoá và tạo lại database.");
            await db.Database.EnsureDeletedAsync();
        }

        if (db.Database.GetMigrations().Any())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            var created = await db.Database.EnsureCreatedAsync();
            if (!created && app.Environment.IsDevelopment())
            {
                bool schemaOk;
                try { await db.Sessions.AnyAsync(); schemaOk = true; }   // bảng mới nhất của S1
                catch { schemaOk = false; }

                if (!schemaOk)
                {
                    logger.LogWarning("DB hiện có thiếu bảng (schema drift) -> tự tạo lại từ model [Development].");
                    await db.Database.EnsureDeletedAsync();
                    await db.Database.EnsureCreatedAsync();
                }
            }
        }

        // Seed Roles
        if (!await db.Roles.AnyAsync(r => r.Code == "ADMIN"))
        {
            db.Roles.Add(new FptConnect.Domain.Entities.Role
            {
                PublicId = Guid.NewGuid(), Code = "ADMIN", Name = "Administrator",
                IsSystem = true, Version = 1, CreatedAtUtc = DateTime.UtcNow
            });
            logger.LogInformation("Seeded role ADMIN");
        }
        if (!await db.Roles.AnyAsync(r => r.Code == "SALE"))
        {
            db.Roles.Add(new FptConnect.Domain.Entities.Role
            {
                PublicId = Guid.NewGuid(), Code = "SALE", Name = "Sale",
                IsSystem = true, Version = 1, CreatedAtUtc = DateTime.UtcNow
            });
            logger.LogInformation("Seeded role SALE");
        }
        await db.SaveChangesAsync();

        // Seed Admin user
        var adminEmail = "admin@fptconnect.vn";
        var adminUser = await db.Users.FirstOrDefaultAsync(u => u.EmailNormalized == adminEmail);
        if (adminUser is null)
        {
            adminUser = new FptConnect.Domain.Entities.User
            {
                PublicId = Guid.NewGuid(),
                EmployeeCode = "EMP0001",
                FullName = "Quan tri vien",
                EmailNormalized = adminEmail,
                PasswordHash = hasher.Hash("Admin@12345"),
                Status = "Active",
                CreatedAtUtc = DateTime.UtcNow
            };
            db.Users.Add(adminUser);
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded admin user: {Email}", adminEmail);
        }
        else
        {
            // Cập nhật password hash nếu user đã tồn tại (đảm bảo hash luôn đúng)
            adminUser.PasswordHash = hasher.Hash("Admin@12345");
            await db.SaveChangesAsync();
            logger.LogInformation("Updated admin password hash: {Email}", adminEmail);
        }

        // Gán role ADMIN cho admin user
        var adminRole = await db.Roles.FirstAsync(r => r.Code == "ADMIN");
        if (!await db.UserRoles.AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id))
        {
            db.UserRoles.Add(new FptConnect.Domain.Entities.UserRole
            {
                UserId = adminUser.Id, RoleId = adminRole.Id
            });
            await db.SaveChangesAsync();
            logger.LogInformation("Assigned ADMIN role to {Email}", adminEmail);
        }

        // Demo data phong phú (chỉ Development): 5 roles + ~33 users + 200 customers VN thật.
        if (app.Environment.IsDevelopment())
            await FptConnect.Infrastructure.Persistence.DevDataSeeder.SeedAsync(db, hasher, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Auto-seed/migrate FAILED: {Message}", ex.Message);
    }
}
// ── End auto-seed ─────────────────────────────────────────────────────

app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(CorsPolicy);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }

