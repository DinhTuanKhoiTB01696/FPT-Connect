using FptConnect.Domain.Entities;
using FptConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptConnect.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/customers")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;
    public CustomersController(AppDbContext db) => _db = db;

    /// <summary>API-023 list (rút gọn Sprint 0): cursor pagination đơn giản, soft-delete đã lọc.</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] int limit = 50)
    {
        limit = Math.Clamp(limit, 1, 100);
        var query = _db.Customers.AsNoTracking().OrderByDescending(c => c.CreatedAtUtc).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(c => c.StatusCode == status);

        var items = await query.Take(limit).Select(c => new
        {
            id = c.PublicId,
            c.FullName,
            c.StatusCode,
            c.PhoneE164,
            c.Address,
            c.CreatedAtUtc
        }).ToListAsync();

        return Ok(new { data = items, meta = new { count = items.Count } });
    }

    public record CreateCustomerRequest(string FullName, string? Phone, string? SourceCode, string? Address);

    /// <summary>API-024 create (rút gọn Sprint 0).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.FullName) || req.FullName.Length is < 2 or > 200)
            return UnprocessableEntity(new { code = "VALIDATION_FAILED", errors = new { fullName = "FULLNAME_INVALID" } });

        var customer = new Customer
        {
            FullName = req.FullName.Trim(),
            PhoneE164 = req.Phone,
            SourceCode = req.SourceCode ?? "MANUAL",
            Address = req.Address,
            StatusCode = "New"
        };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(List), new { id = customer.PublicId }, new { data = new { id = customer.PublicId } });
    }
}
