using FptConnect.Application.Common;

namespace FptConnect.Api.Security;

/// <summary>Đọc danh tính từ JWT claim ("uid", "sid") cho request hiện tại.</summary>
public sealed class CurrentUser : ICurrentUser
{
    public long? UserId { get; }
    public long? SessionId { get; }

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User;
        if (long.TryParse(user?.FindFirst("uid")?.Value, out var uid)) UserId = uid;
        if (long.TryParse(user?.FindFirst("sid")?.Value, out var sid)) SessionId = sid;
    }
}
