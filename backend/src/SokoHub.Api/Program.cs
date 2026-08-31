using SokoHub.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "SokoHub API");

app.Run();
