using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using Etrx.API.Security;
using Etrx.Application.Constants;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Etrx.API.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddEtrxAuthentication(this IServiceCollection services,
        IConfiguration configuration, bool development)
    {
        var limits = configuration.GetRequiredSection(nameof(AuthenticationLimits)).Get<AuthenticationLimits>()!;
        limits.Validate();
        services.AddSingleton(limits);

        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        if (development && string.IsNullOrWhiteSpace(jwt.SigningKey))
        {
            jwt.SigningKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(Math.Max(limits.MinimumSigningKeyBytes, limits.RandomTokenBytes)));
        }

        if (string.IsNullOrWhiteSpace(jwt.SigningKey) || Encoding.UTF8.GetByteCount(jwt.SigningKey) < limits.MinimumSigningKeyBytes ||
            string.IsNullOrWhiteSpace(jwt.Issuer) || string.IsNullOrWhiteSpace(jwt.Audience) ||
            jwt.AccessTokenMinutes < limits.MinimumAccessTokenMinutes || jwt.AccessTokenMinutes > limits.MaximumAccessTokenMinutes ||
            jwt.RefreshTokenHours < limits.MinimumRefreshTokenHours || jwt.RefreshTokenHours > limits.MaximumRefreshTokenHours)
        {
            throw new InvalidOperationException($"Configure Jwt:SigningKey (at least {limits.MinimumSigningKeyBytes} bytes), Issuer, Audience and valid token lifetimes.");
        }

        services.AddSingleton<IOptions<JwtOptions>>(Options.Create(jwt));
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
            options.Password.RequiredLength = RequestLimits.MinPasswordLength;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Lockout.MaxFailedAccessAttempts = RequestLimits.MaxFailedLoginAttempts;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(RequestLimits.LoginLockoutMinutes);
            options.Lockout.AllowedForNewUsers = true;
        }).AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<EtrxDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
        services.AddScoped<IPasswordHasher<ApplicationUser>, MigratingPasswordHasher>();
        services.AddScoped<JwtPrincipalValidator>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, ValidIssuer = jwt.Issuer,
                ValidateAudience = true, ValidAudience = jwt.Audience,
                ValidateLifetime = true, RequireExpirationTime = true,
                ValidateIssuerSigningKey = true, RequireSignedTokens = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                ClockSkew = TimeSpan.FromSeconds(limits.JwtClockSkewSeconds),
                NameClaimType = ClaimTypes.Name, RoleClaimType = ClaimTypes.Role
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context => context.HttpContext.RequestServices
                    .GetRequiredService<JwtPrincipalValidator>().ValidateAsync(context)
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
                { PermitLimit = limits.AuthRequestsPerWindow, Window = TimeSpan.FromMinutes(limits.RateLimitWindowMinutes), QueueLimit = 0 }));
        });
        return services;
    }
}
