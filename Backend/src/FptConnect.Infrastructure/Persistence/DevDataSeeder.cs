using System.Security.Cryptography;
using System.Text;
using FptConnect.Application.Common;
using FptConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FptConnect.Infrastructure.Persistence;

/// <summary>
/// Seed dữ liệu demo "giống production" cho môi trường Development (Bible: không bao giờ khởi động với DB rỗng).
/// Phạm vi hiện tại theo schema đã hiện thực: Roles, Users (đủ 5 vai trò), Customers (VN thật, GPS quanh HCM/Bình Dương/Đồng Nai...).
/// Idempotent: chỉ seed khi bảng trống. Dùng bulk AddRange + một SaveChanges.
/// </summary>
public static class DevDataSeeder
{
    private const string DemoPassword = "Demo@12345"; // mật khẩu chung cho user demo (không phải tài khoản admin chính)

    private static readonly string[] HoList =
        { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng", "Bùi", "Đỗ", "Hồ", "Ngô", "Dương", "Lý" };
    private static readonly string[] TenDem =
        { "Văn", "Thị", "Hữu", "Đức", "Minh", "Thanh", "Quang", "Hồng", "Ngọc", "Gia", "Anh", "Thành", "Tuấn", "Khánh", "Bảo", "Kim" };
    private static readonly string[] TenList =
        { "An", "Bình", "Cường", "Dũng", "Phương", "Giang", "Hà", "Hải", "Khoa", "Lan", "Mai", "Nam", "Oanh", "Phúc", "Quân",
          "Sơn", "Trang", "Uyên", "Vy", "Yến", "Long", "Tâm", "Thảo", "Hùng", "Linh", "Ngân", "Tú", "Đạt", "Hiếu", "Như" };

    // Khu vực + tâm toạ độ (xấp xỉ trung tâm) + một số đường thật
    private static readonly (string Area, double Lat, double Lng, string[] Streets)[] Areas =
    {
        ("Quận 1, TP.HCM",        10.7769, 106.7009, new[] { "Nguyễn Huệ", "Lê Lợi", "Hai Bà Trưng", "Pasteur", "Đồng Khởi" }),
        ("Quận 3, TP.HCM",        10.7798, 106.6850, new[] { "Võ Văn Tần", "Nguyễn Đình Chiểu", "Cách Mạng Tháng 8", "Lý Chính Thắng" }),
        ("Thủ Đức, TP.HCM",       10.8490, 106.7710, new[] { "Võ Văn Ngân", "Kha Vạn Cân", "Đặng Văn Bi", "Lê Văn Việt" }),
        ("Thủ Dầu Một, Bình Dương",10.9804,106.6519, new[] { "Đại lộ Bình Dương", "Cách Mạng Tháng 8", "Yersin", "Huỳnh Văn Nghệ" }),
        ("Dĩ An, Bình Dương",     10.9060, 106.7690, new[] { "Lý Thường Kiệt", "Nguyễn An Ninh", "Trần Hưng Đạo", "Lê Trọng Tấn" }),
        ("Thuận An, Bình Dương",  10.9170, 106.7140, new[] { "Đại lộ Bình Dương", "Nguyễn Văn Tiết", "Cách Mạng Tháng 8" }),
        ("Biên Hòa, Đồng Nai",    10.9574, 106.8426, new[] { "Phạm Văn Thuận", "Đồng Khởi", "Nguyễn Ái Quốc", "Võ Thị Sáu" }),
        ("Long Khánh, Đồng Nai",  10.9300, 107.2400, new[] { "Hùng Vương", "Cách Mạng Tháng 8", "Nguyễn Trãi" }),
    };

    private static readonly string[] Statuses =
        { "New", "Contacted", "Interested", "Negotiating", "Signed", "Completed", "Cancelled" };
    private static readonly string[] Sources =
        { "FIELD_SURVEY", "HOTLINE", "REFERRAL", "FANPAGE", "WEBSITE", "EVENT" };

    public static async Task SeedAsync(AppDbContext db, IPasswordHasher hasher, ILogger logger, CancellationToken ct = default)
    {
        var rnd = new Random(20260612);

        await EnsureRolesAsync(db, ct);
        await EnsureUsersAsync(db, hasher, rnd, logger, ct);
        await EnsureCustomersAsync(db, rnd, logger, ct);
    }

    private static async Task EnsureRolesAsync(AppDbContext db, CancellationToken ct)
    {
        var roleDefs = new (string Code, string Name)[]
        {
            ("SUPERADMIN", "Super Admin"), ("ADMIN", "Administrator"),
            ("MANAGER", "Manager"), ("SALE", "Sale"), ("TECHNICIAN", "Technician")
        };
        foreach (var (code, name) in roleDefs)
        {
            if (!await db.Roles.AnyAsync(r => r.Code == code, ct))
                db.Roles.Add(new Role { PublicId = Guid.NewGuid(), Code = code, Name = name, IsSystem = true, Version = 1, CreatedAtUtc = DateTime.UtcNow });
        }
        await db.SaveChangesAsync(ct);
    }

    private static async Task EnsureUsersAsync(AppDbContext db, IPasswordHasher hasher, Random rnd, ILogger logger, CancellationToken ct)
    {
        // Đã có nhiều user => bỏ qua (giữ idempotent; admin@fptconnect.vn do auto-seed tạo riêng)
        if (await db.Users.CountAsync(ct) >= 30) return;

        var roles = await db.Roles.ToDictionaryAsync(r => r.Code, r => r.Id, ct);
        var hash = hasher.Hash(DemoPassword);
        var usedEmails = new HashSet<string>(await db.Users.Select(u => u.EmailNormalized).ToListAsync(ct));
        var usedCodes = new HashSet<string>(await db.Users.Select(u => u.EmployeeCode).ToListAsync(ct));

        var newUsers = new List<(User User, string RoleCode)>();
        var plan = new (string RoleCode, int Count, string Prefix)[]
        {
            ("SUPERADMIN", 1, "SA"), ("ADMIN", 2, "AD"), ("MANAGER", 5, "MG"), ("SALE", 15, "SL"), ("TECHNICIAN", 10, "TC")
        };

        var seq = 100;
        foreach (var (roleCode, count, prefix) in plan)
        {
            for (var i = 0; i < count; i++)
            {
                var fullName = RandomName(rnd);
                string email, code;
                do { email = Slug(fullName) + "." + prefix.ToLower() + (++seq) + "@fptconnect.vn"; } while (!usedEmails.Add(email));
                do { code = "EMP" + (1000 + seq); } while (!usedCodes.Add(code));

                newUsers.Add((new User
                {
                    PublicId = Guid.NewGuid(),
                    EmployeeCode = code,
                    FullName = fullName,
                    EmailNormalized = email,
                    PasswordHash = hash,
                    Status = "Active",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-rnd.Next(1, 400))
                }, roleCode));
            }
        }

        db.Users.AddRange(newUsers.Select(x => x.User));
        await db.SaveChangesAsync(ct);

        db.UserRoles.AddRange(newUsers
            .Where(x => roles.ContainsKey(x.RoleCode))
            .Select(x => new UserRole { UserId = x.User.Id, RoleId = roles[x.RoleCode] }));
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Seeded {Count} demo users (mật khẩu demo: {Pwd})", newUsers.Count, DemoPassword);
    }

    private static async Task EnsureCustomersAsync(AppDbContext db, Random rnd, ILogger logger, CancellationToken ct)
    {
        if (await db.Customers.IgnoreQueryFilters().AnyAsync(ct)) return; // đã có khách hàng

        var saleIds = await (from ur in db.UserRoles
                             join r in db.Roles on ur.RoleId equals r.Id
                             where r.Code == "SALE"
                             select ur.UserId).ToListAsync(ct);

        var usedPhones = new HashSet<string>();
        var list = new List<Customer>(200);

        for (var i = 0; i < 200; i++)
        {
            var area = Areas[rnd.Next(Areas.Length)];
            var fullName = RandomName(rnd);

            string phone;
            do { phone = "+849" + rnd.Next(0, 99999999).ToString("D8"); } while (!usedPhones.Add(phone));

            var lat = Math.Round(area.Lat + (rnd.NextDouble() - 0.5) * 0.06, 6);   // ~ ±3 km
            var lng = Math.Round(area.Lng + (rnd.NextDouble() - 0.5) * 0.06, 6);
            var houseNo = rnd.Next(1, 400);
            var street = area.Streets[rnd.Next(area.Streets.Length)];

            list.Add(new Customer
            {
                PublicId = Guid.NewGuid(),
                FullName = fullName,
                PhoneE164 = phone,
                PhoneHash = SHA256.HashData(Encoding.UTF8.GetBytes(phone)),
                Email = Slug(fullName) + (i + 1) + "@gmail.com",
                StatusCode = Statuses[rnd.Next(Statuses.Length)],
                SourceCode = Sources[rnd.Next(Sources.Length)],
                Address = $"{houseNo} {street}, {area.Area}",
                Latitude = (decimal)lat,
                Longitude = (decimal)lng,
                OwnerUserId = saleIds.Count > 0 ? saleIds[rnd.Next(saleIds.Count)] : (long?)null,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-rnd.Next(0, 180)).AddMinutes(-rnd.Next(0, 1440))
            });
        }

        db.Customers.AddRange(list); // bulk insert (EF batch)
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} demo customers across {Areas} areas", list.Count, Areas.Length);
    }

    private static string RandomName(Random rnd) =>
        $"{HoList[rnd.Next(HoList.Length)]} {TenDem[rnd.Next(TenDem.Length)]} {TenList[rnd.Next(TenList.Length)]}";

    /// <summary>Bỏ dấu tiếng Việt + tạo slug cho email.</summary>
    private static string Slug(string name)
    {
        var normalized = name.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var cat = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(System.Text.NormalizationForm.FormC)
            .ToLowerInvariant().Replace("đ", "d").Replace(" ", ".");
    }
}
