using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Domain.Interfaces;
using SokoHub.Infrastructure.Cache.Redis;
using SokoHub.Infrastructure.Common;
using SokoHub.Infrastructure.Identity.Jwt;
using SokoHub.Infrastructure.Identity.PasswordHashing;
using SokoHub.Infrastructure.Messaging.RabbitMq;
using SokoHub.Infrastructure.Persistence.Mssql;
using SokoHub.Infrastructure.Payments.Mpesa;
using SokoHub.Infrastructure.Payments.Mpesa.Daraja;
using SokoHub.Infrastructure.Payments.Mpesa.StkPush;
using SokoHub.Infrastructure.Payments.Mpesa.StkQuery;
using SokoHub.Infrastructure.Payments.Mpesa.B2C;
using SokoHub.Infrastructure.Payments.Mpesa.Callbacks;
using SokoHub.Infrastructure.Payments.Mpesa.Webhooks;
using SokoHub.Infrastructure.Payments.Mpesa.Reversal;
using SokoHub.Infrastructure.Payments.Mpesa.TransactionStatus;
using SokoHub.Infrastructure.Payments.Mpesa.Idempotency;
using StackExchange.Redis;

namespace SokoHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddCommonServices(services);
        AddPersistenceServices(services, configuration);
        AddIdentityServices(services);
        AddCacheServices(services, configuration);
        AddMessagingServices(services, configuration);
        AddPaymentServices(services, configuration);

        return services;
    }

    private static void AddCommonServices(IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddTransient<IdempotencyService>();
    }

    private static void AddPersistenceServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SokoHubDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, MssqlUnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(MssqlRepository<>));
    }

    private static void AddIdentityServices(IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtProvider, JwtTokenService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var logger = sp.GetRequiredService<ILogger<JwtTokenService>>();
            return new JwtTokenService(config, logger);
        });
    }

    private static void AddCacheServices(IServiceCollection services, IConfiguration configuration)
    {
        var redisSection = configuration.GetSection("Redis");
        var connectionString = redisSection["ConnectionString"] ?? "localhost:6379";

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(connectionString));

        services.AddScoped<ICacheService, RedisCacheService>();
    }

    private static void AddMessagingServices(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitSection = configuration.GetSection("RabbitMQ");
        var host = rabbitSection["Host"] ?? "localhost";
        var user = rabbitSection["User"] ?? "guest";
        var pass = rabbitSection["Password"] ?? "guest";

        var factory = new ConnectionFactory
        {
            HostName = host,
            UserName = user,
            Password = pass,
            AutomaticRecoveryEnabled = true,
            DispatchConsumersAsync = true,
            TopologyRecoveryEnabled = true,
        };

        services.AddSingleton<IConnection>(sp => factory.CreateConnection());
        services.AddSingleton<IEventBus, RabbitMqBus>();
    }

    private static void AddPaymentServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IMpesaService, MpesaService>(client =>
        {
            client.BaseAddress = new Uri(configuration["Mpesa:DarajaUrl"] ?? "https://sandbox.safaricom.co.ke");
        });

        services.AddTransient<DarajaAuthenticationClient>();
        services.AddTransient<StkPushService>();
        services.AddTransient<StkQueryService>();
        services.AddTransient<B2cService>();
        services.AddTransient<MpesaCallbackProcessor>();
        services.AddTransient<MpesaWebhookProcessor>();
        services.AddTransient<MpesaReversalService>();
        services.AddTransient<MpesaTransactionStatusService>();
        services.AddTransient<MpesaIdempotencyService>();
    }
}
