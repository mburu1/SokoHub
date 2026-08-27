var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "SokoHub API");

app.Run();