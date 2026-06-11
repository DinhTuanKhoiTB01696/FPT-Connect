using FptConnect.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace FptConnect.Domain.Tests;

public class CustomerTests
{
    [Fact]
    public void New_customer_defaults_to_New_status_and_not_deleted()
    {
        var c = new Customer { FullName = "Nguyen Van An" };
        c.StatusCode.Should().Be("New");
        c.IsDeleted.Should().BeFalse();
        c.PublicId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Soft_delete_fields_support_restore_audit()
    {
        var c = new Customer { FullName = "Tran Binh", IsDeleted = true, DeletedAtUtc = DateTime.UtcNow, DeleteReason = "merge" };
        c.IsDeleted.Should().BeTrue();
        c.DeletedAtUtc.Should().NotBeNull();
        c.DeleteReason.Should().Be("merge");
    }
}
