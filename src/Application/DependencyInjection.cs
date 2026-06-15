using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FptConnect.Application;

/// <summary>Đăng ký tầng Application (CQRS pipeline). Chưa có handler — sẽ thêm theo từng feature/sprint.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
