using System.Net;
using System.Text.Json;
using Etrx.Application.Exceptions;
using Etrx.Application.Dtos.Common;

namespace Etrx.API.Middlewares;

public class GlobalExceptionMiddleware
{
    public const string CONTENT_TYPE = "application/json";

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = CONTENT_TYPE;
        
        int statusCode;
        string jsonResponse;

        switch (exception)
        {
            case NotFoundException notFoundEx:
                statusCode = (int)HttpStatusCode.NotFound;

                var notFoundResponse = new ErrorResponseDto<string>(
                    notFoundEx.Message,
                    statusCode);

                jsonResponse = JsonSerializer.Serialize(notFoundResponse);
                break;

            case CodeforcesApiException cfApiEx:
                statusCode = (int)HttpStatusCode.InternalServerError;

                var cfApiResponse = new ErrorResponseDto<string>(
                    cfApiEx.Message,
                    statusCode);

                jsonResponse = JsonSerializer.Serialize(cfApiResponse);
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;

                var internalErrorResponse = new ErrorResponseDto<string>(
                    "An internal server error has occurred",
                    statusCode);

                jsonResponse = JsonSerializer.Serialize(internalErrorResponse);
                break;
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(jsonResponse);
    }
}
