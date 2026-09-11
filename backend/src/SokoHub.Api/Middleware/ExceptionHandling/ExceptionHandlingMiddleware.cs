using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SokoHub.Application.Common.Results;

namespace SokoHub.Api.Middleware.ExceptionHandling;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var problem = ProduceProblem(exception);
        response.StatusCode = problem.Status;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(problem, options);

        await response.WriteAsync(json);
    }

    private ProblemDetails ProduceProblem(Exception exception)
    {
        return exception switch
        {
            ValidationException vex => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Detail = "One or more validation errors occurred.",
                Extensions = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["errors"] = vex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage, e.AttemptedValue, e.ErrorCode })
                }
            },
            _ => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = _env.IsDevelopment() ? exception.Message : "An internal server error occurred.",
                Detail = _env.IsDevelopment() ? exception.StackTrace : null
            }
        };
    }
}
