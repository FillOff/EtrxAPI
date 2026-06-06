using Etrx.Application.Dtos.Common;
using Etrx.Application.Exceptions.Api;
using Etrx.Application.Exceptions.BadRequest;
using Etrx.Application.Exceptions.NotFound;
using System.Net;
using System.Text.Json;

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
            case BadRequestException badRequestEx:
                statusCode = (int)HttpStatusCode.BadRequest;

                var inLangResponse = new ErrorResponseDto<string>(
                    badRequestEx.Message,
                    statusCode);

                jsonResponse = JsonSerializer.Serialize(inLangResponse);
                break;

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
