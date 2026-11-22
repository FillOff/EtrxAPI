using Etrx.Application.Interfaces;
using Etrx.Application.Dtos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Etrx.Application.Constants;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequestDto dto)
    {
        var user = await _usersService.CreateUserAsync(dto);

        return Ok();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetUsersWithSortAsync(
        [FromQuery] GetSortUserRequestDto dto)
    {
        return Ok(await _usersService.GetUsersWithSortAsync(dto));
    }

    [HttpGet("{handle}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserByHandleAsync(string handle)
    {
        return Ok(await _usersService.GetUserByHandleAsync(handle));
    }

    [HttpDelete("{handle}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> DeleteUserByHandleAsync(string handle)
    {
        await _usersService.DeleteUserByHandleAsync(handle);

        return NoContent();
    }
}
