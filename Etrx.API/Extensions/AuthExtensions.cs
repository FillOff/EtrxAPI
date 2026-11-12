using System.Security.Claims;
using System.Threading.RateLimiting;
using Etrx.API.Security;
using Etrx.Application.Constants;
using Etrx.Application.Options;
using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Etrx.API.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddEtrxAuthentication(this IServiceCollection services, bool development)
    {
        services.AddScoped<IPasswordHasher<UserCredential>, PasswordHasher<UserCredential>>();
        services.Configure<PasswordHasherOptions>(options => options.IterationCount = 210_000);
        services.AddAuthentication(AuthSecurity.Scheme).AddCookie(AuthSecurity.Scheme, options =>
        {
            options.Cookie.Name = "etrx.session";
            options.Cookie.HttpOnly = true;
            options.Cookie.Path = "/";
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = development ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
            options.ExpireTimeSpan = TimeSpan.FromHours(12);
            options.SlidingExpiration = false;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
            options.Events.OnValidatePrincipal = async context =>
            {
                if (!Guid.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                {
                    context.RejectPrincipal();
                    return;
                }

                var repository = context.HttpContext.RequestServices.GetRequiredService<IAuthRepository>();
                var account = await repository.FindCredentialAsync(id);
                var user = await repository.FindUserByIdAsync(id, context.HttpContext.RequestAborted);
                if (account is null || user is null || account.SecurityStamp != context.Principal?.FindFirstValue(AuthSecurity.SecurityStampClaim))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(AuthSecurity.Scheme);
                    return;
                }

                var authOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AuthOptions>>().Value;
                context.ReplacePrincipal(AuthSecurity.CreatePrincipal(user, account, authOptions));
            };
        });
        services.AddAuthorization(options => options.AddPolicy(AuthorizationPolicies.Admin,
            policy => policy.RequireRole(AuthorizationPolicies.Admin)));
        services.AddCors();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(RateLimitPolicies.Auth, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
                { PermitLimit = 20, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
        });
        return services;
    }
}
