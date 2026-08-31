using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SokoHub.Application;
using SokoHub.Infrastructure;

namespace SokoHub.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}
