using Microsoft.Extensions.DependencyInjection;
using SokoHub.Application.Auth;
using System.Reflection;

namespace SokoHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
