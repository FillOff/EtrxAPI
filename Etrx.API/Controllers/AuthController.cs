using Etrx.Application.Constants;
using Etrx.Application.Dtos.Auth;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("user-queries"), AllowAnonymous, EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> LookupAsync(HandleRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _authService.LookupAsync(request, cancellationToken));
    }

    [HttpPost("credentials"), AllowAnonymous, EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> SetupAsync(PasswordRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _authService.SetupAsync(request, cancellationToken));
    }

    [HttpPost("sessions"), AllowAnonymous, EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> LoginAsync(PasswordRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _authService.LoginAsync(request, cancellationToken));
    }

    [HttpGet("session"), AllowAnonymous]
    public IActionResult GetSession()
    {
        Response.Headers.CacheControl = "no-store";
        return Ok(_authService.GetSession());
    }

    [HttpPost("tokens/refresh"), AllowAnonymous, EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> RefreshAsync(CancellationToken cancellationToken)
    {
        return Ok(await _authService.RefreshAsync(cancellationToken));
    }

    [HttpDelete("session"), AllowAnonymous]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(cancellationToken);
        return NoContent();
    }
}
