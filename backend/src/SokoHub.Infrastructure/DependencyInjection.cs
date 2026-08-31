using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Domain.Interfaces;
using SokoHub.Infrastructure.Identity.Jwt;
using SokoHub.Infrastructure.Identity.PasswordHashing;
using SokoHub.Infrastructure.Persistence.Mssql;

namespace SokoHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<SokoHubDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Identity Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtTokenService>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, MssqlUnitOfWork>();

        return services;
    }
}
