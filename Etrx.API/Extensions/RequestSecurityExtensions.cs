using Etrx.Application.Options;
using Microsoft.Extensions.Options;

namespace Etrx.API.Extensions;

public static class RequestSecurityExtensions
{
    private const string ApiPath = "/api";
    private const string RequestHeader = "X-ETRX-Request";
    private const string RequestHeaderValue = "1";
    private const string LocalFrontendOrigin = "http://localhost:3000";

    public static IApplicationBuilder UseFrontendCors(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<AuthOptions>>().Value;
        var environment = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
        var origins = options.AllowedOrigins ?? [];
        if (origins.Length == 0 && environment.IsDevelopment())
        {
            origins = [LocalFrontendOrigin];
        }

        return app.UseCors(policy =>
        {
            if (origins.Length > 0)
            {
                policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            }
        });
    }

    public static IApplicationBuilder UseApiRequestHeader(this IApplicationBuilder app) => app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments(ApiPath) && !HttpMethods.IsGet(context.Request.Method) &&
            !HttpMethods.IsHead(context.Request.Method) && !HttpMethods.IsOptions(context.Request.Method) &&
            context.Request.Headers[RequestHeader] != RequestHeaderValue)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await next(context);
    });
}
