using System.Security.Claims;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Auth;
using Etrx.Application.Interfaces;

namespace Etrx.API.Services;

public sealed class AuthSession : IAuthSession
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public Guid? UserId => Principal.Identity?.IsAuthenticated == true &&
        Guid.TryParse(Principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public SessionResponseDto Current => new SessionResponseDto(Principal.Identity?.IsAuthenticated == true,
        Principal.Identity?.Name, Principal.IsInRole(AuthorizationPolicies.Admin));

    public AuthSession(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}
