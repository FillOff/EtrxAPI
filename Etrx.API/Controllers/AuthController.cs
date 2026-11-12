using Etrx.Application.Dtos.Auth;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Common;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("user-queries"), AllowAnonymous, EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> Lookup(HandleRequestDto request) => Respond(await authService.LookupAsync(request));

    [HttpPost("credentials"), AllowAnonymous, EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> Setup(PasswordRequestDto request) => Respond(await authService.SetupAsync(request));

    [HttpPost("sessions"), AllowAnonymous, EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> Login(PasswordRequestDto request) => Respond(await authService.LoginAsync(request));

    [HttpGet("session"), AllowAnonymous]
    public IActionResult Me() => Ok(authService.CurrentSession());

    [HttpDelete("session"), AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();
        return NoContent();
    }

    private IActionResult Respond<T>(AuthResult<T> result)
    {
        if (result.Status == AuthStatus.Success)
        {
            return Ok(result.Value);
        }

        var statusCode = result.Status switch
        {
            AuthStatus.NotFound => StatusCodes.Status404NotFound,
            AuthStatus.BadRequest => StatusCodes.Status400BadRequest,
            AuthStatus.Conflict => StatusCodes.Status409Conflict,
            AuthStatus.Forbidden => StatusCodes.Status403Forbidden,
            AuthStatus.Unauthorized => StatusCodes.Status401Unauthorized,
            AuthStatus.TooManyRequests => StatusCodes.Status429TooManyRequests,
            _ => StatusCodes.Status500InternalServerError
        };
        return StatusCode(statusCode, new MessageResponseDto(result.Message ?? "Authentication failed."));
    }
}
