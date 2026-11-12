using System.Security.Claims;
using Etrx.API.Security;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Auth;
using Etrx.Application.Interfaces;
using Etrx.Application.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Etrx.API.Services;

public sealed class AuthSession(IHttpContextAccessor accessor, IOptions<AuthOptions> options) : IAuthSession
{
    private HttpContext Context => accessor.HttpContext ?? throw new InvalidOperationException("No active HTTP context.");
    public Guid? UserId => Guid.TryParse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public SessionResponseDto Current => new(Context.User.Identity?.IsAuthenticated == true,
        Context.User.Identity?.Name, Context.User.IsInRole(AuthorizationPolicies.Admin));

    public Task SignInAsync(AuthenticatedUser user) => Context.SignInAsync(AuthSecurity.Scheme,
        AuthSecurity.CreatePrincipal(user.User, user.Credential, options.Value),
        new AuthenticationProperties { IsPersistent = true });

    public Task SignOutAsync() => Context.SignOutAsync(AuthSecurity.Scheme);
}
