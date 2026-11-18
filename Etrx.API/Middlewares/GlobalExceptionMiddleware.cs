using Etrx.Application.Exceptions;
using Etrx.Application.Exceptions.Api;
using Etrx.Application.Exceptions.BadRequest;
using Etrx.Application.Exceptions.NotFound;
using FluentValidation;

namespace Etrx.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The caller disconnected; do not turn cancellation into a server error.
        }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            var statusCode = exception switch
            {
                BadRequestException or ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                TooManyRequestsException => StatusCodes.Status429TooManyRequests,
                _ => StatusCodes.Status500InternalServerError
            };
            if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                    context.Request.Method, context.Request.Path);
            }
            else
            {
                _logger.LogWarning("Request rejected with {StatusCode}: {Message}", statusCode, exception.Message);
            }

            var message = exception is RequestException or ValidationException or CodeforcesApiException
                ? exception.Message : "An internal server error has occurred";
            context.Response.StatusCode = statusCode;
            // Preserve the existing Errors/StatusCode contract and the message read by auth clients.
            await context.Response.WriteAsJsonAsync(new { Errors = message, StatusCode = statusCode, message },
                new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = null },
                context.RequestAborted);
        }
    }
}
