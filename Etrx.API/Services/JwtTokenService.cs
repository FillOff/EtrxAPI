using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Etrx.API.Security;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Auth;
using Etrx.Application.Exceptions;
using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Etrx.API.Services;

public sealed class JwtTokenService
{
    public const string RefreshCookieName = "etrx.refresh";
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtOptions _jwtOptions;
    private readonly IHostEnvironment _environment;

    private HttpContext Context => _httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("No active HTTP context.");

    public JwtTokenService(IRefreshTokenRepository refreshTokenRepository, IUsersRepository usersRepository,
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IHttpContextAccessor httpContextAccessor, IOptions<JwtOptions> jwtOptions, IHostEnvironment environment)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _usersRepository = usersRepository;
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
        _environment = environment;
    }

    public async Task<(SessionResponseDto Session, RefreshToken Token, string Secret)> CreateAsync(
        ApplicationUser user, CancellationToken cancellationToken)
    {
        var issuedTokens = await PrepareAsync(user, DateTimeOffset.UtcNow.AddHours(_jwtOptions.RefreshTokenHours), cancellationToken);
        await _refreshTokenRepository.AddAsync(issuedTokens.Token, cancellationToken);
        return issuedTokens;
    }

    public SessionResponseDto CompleteSignIn((SessionResponseDto Session, RefreshToken Token, string Secret) issuedTokens)
    {
        // Call only after the surrounding account transaction has committed.
        Context.Response.Cookies.Append(RefreshCookieName, issuedTokens.Secret, CreateCookieOptions(issuedTokens.Token.ExpiresAt));
        Context.Response.Cookies.Delete("etrx.session", new CookieOptions { Path = "/" });
        Context.Response.Headers.CacheControl = "no-store";
        Context.Response.Headers.Pragma = "no-cache";
        return issuedTokens.Session;
    }

    public async Task<SessionResponseDto> RefreshAsync(CancellationToken cancellationToken)
    {
        var stored = await ReadCookieAsync(cancellationToken);
        if (stored is null || stored.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            throw new UnauthorizedException("Refresh token is invalid or expired. Sign in again.");
        }
        var user = await _userManager.FindByIdAsync(stored.UserId.ToString());
        if (user is null || stored.SecurityStamp != await _userManager.GetSecurityStampAsync(user))
        {
            throw new UnauthorizedException("Session has been revoked. Sign in again.");
        }
        if (stored.RevokedAt is not null)
        {
            await RevokeAccountAsync(user);
            throw new UnauthorizedException("Refresh token was already used. Sign in again.");
        }
        if (await _userManager.IsLockedOutAsync(user) || !await _signInManager.CanSignInAsync(user) ||
            await _userManager.GetTwoFactorEnabledAsync(user))
        {
            throw new UnauthorizedException("Account is unavailable.");
        }
        var issuedTokens = await PrepareAsync(user, stored.ExpiresAt, cancellationToken);
        if (!await _refreshTokenRepository.TryRotateAsync(stored, issuedTokens.Token, cancellationToken))
        {
            var current = await _userManager.FindByIdAsync(user.Id.ToString());
            if (current is not null && stored.SecurityStamp == await _userManager.GetSecurityStampAsync(current))
            {
                await RevokeAccountAsync(current);
            }
            throw new UnauthorizedException("Refresh token was already used. Sign in again.");
        }
        return CompleteSignIn(issuedTokens);
    }

    public async Task RevokeAsync(Guid? authenticatedUserId, CancellationToken cancellationToken)
    {
        var stored = await ReadCookieAsync(cancellationToken);
        var userId = authenticatedUserId;
        if (userId is null && stored is { RevokedAt: null } && stored.ExpiresAt > DateTimeOffset.UtcNow)
        {
            var cookieUser = await _userManager.FindByIdAsync(stored.UserId.ToString());
            if (cookieUser is not null && stored.SecurityStamp == await _userManager.GetSecurityStampAsync(cookieUser))
            {
                userId = cookieUser.Id;
            }
        }
        if (userId is Guid id && await _userManager.FindByIdAsync(id.ToString()) is { } user)
        {
            await RevokeAccountAsync(user);
        }
        Context.Response.Cookies.Delete(RefreshCookieName, CreateCookieOptions());
        Context.Response.Cookies.Delete("etrx.session", new CookieOptions { Path = "/" });
        Context.Response.Headers.CacheControl = "no-store";
    }

    private async Task<(SessionResponseDto Session, RefreshToken Token, string Secret)> PrepareAsync(
        ApplicationUser user, DateTimeOffset refreshExpiresAt, CancellationToken cancellationToken)
    {
        var profile = await _usersRepository.FindByIdAsync(user.Id, cancellationToken)
            ?? throw new UnauthorizedException("Profile no longer exists.");
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        var claims = principal.Claims.Where(claim => claim.Type != ClaimTypes.Name)
            .Append(new Claim(ClaimTypes.Name, profile.Handle))
            .Append(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()))
            .Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
        var token = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims, DateTime.UtcNow, expiresAt.UtcDateTime,
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256));
        var secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(AuthenticationLimits.RandomTokenBytes));
        var stored = new RefreshToken
        {
            TokenHash = Hash(secret), UserId = user.Id, SecurityStamp = await _userManager.GetSecurityStampAsync(user),
            ExpiresAt = refreshExpiresAt
        };
        var session = new SessionResponseDto(true, profile.Handle, principal.IsInRole(AuthorizationPolicies.Admin),
            new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        return (session, stored, secret);
    }

    private async Task RevokeAccountAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateSecurityStampAsync(user);
        if (!result.Succeeded)
        {
            // A simultaneous stamp update already invalidated the same session.
            if (result.Errors.All(error => error.Code == nameof(IdentityErrorDescriber.ConcurrencyFailure)))
            {
                throw new UnauthorizedException("Session has already changed. Sign in again.");
            }
            IdentityRoleInitializer.EnsureSuccess(result);
        }
    }

    private Task<RefreshToken?> ReadCookieAsync(CancellationToken cancellationToken)
    {
        var secret = Context.Request.Cookies[RefreshCookieName];
        return string.IsNullOrWhiteSpace(secret) || secret.Length > AuthenticationLimits.MaximumRefreshTokenLength
            ? Task.FromResult<RefreshToken?>(null) : _refreshTokenRepository.FindAsync(Hash(secret), cancellationToken);
    }

    private CookieOptions CreateCookieOptions(DateTimeOffset? expiresAt = null)
    {
        return new CookieOptions
        {
            HttpOnly = true, Secure = !_environment.IsDevelopment() || Context.Request.IsHttps,
            SameSite = SameSiteMode.Lax, Path = "/", Expires = expiresAt, IsEssential = true
        };
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
