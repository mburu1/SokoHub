using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SokoHub.Infrastructure;
using SokoHub.Application;

var builder = Host.CreateApplicationBuilder(args);

// Add Core Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Worker-specific services
builder.Services.AddHostedService<SokoHub.Workers.WorkerHost>();

var host = builder.Build();
host.Run();
