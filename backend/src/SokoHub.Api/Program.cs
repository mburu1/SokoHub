using Scalar.AspNetCore;
using SokoHub.Api;
using SokoHub.Api.Middleware.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("SokoHubPolicy");

app.MapControllers();

app.MapScalarApiReference();
app.MapGet("/", () => "SokoHub API");

app.Run();
