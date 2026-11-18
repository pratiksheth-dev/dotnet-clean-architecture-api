using CleanArchitecture.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

/// <summary>Registers Application-layer services with the DI container.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductService>();
        services.AddScoped<OrderService>();
        return services;
    }
}
