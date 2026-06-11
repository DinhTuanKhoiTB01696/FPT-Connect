using Microsoft.Extensions.DependencyInjection;

namespace FptConnect.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Đăng ký MediatR/validators ở đây khi mở rộng (Bible 7.5).
        return services;
    }
}
