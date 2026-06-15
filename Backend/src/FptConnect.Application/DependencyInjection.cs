using FptConnect.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace FptConnect.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<SessionService>();
        return services;
    }
}
