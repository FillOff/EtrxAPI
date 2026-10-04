using Etrx.Application.Interfaces;
using Etrx.Application.Dtos.Users;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private const string GetUserByHandleRouteName = "GetUserByHandle";

    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequestDto dto)
    {
        var user = await _usersService.CreateUserAsync(dto);

        return CreatedAtRoute(GetUserByHandleRouteName, new { handle = user.Handle }, user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersWithSortAsync(
        [FromQuery] GetSortUserRequestDto dto)
    {
        return Ok(await _usersService.GetUsersWithSortAsync(dto));
    }

    [HttpGet("{handle}", Name = GetUserByHandleRouteName)]
    public async Task<IActionResult> GetUserByHandleAsync(string handle)
    {
        return Ok(await _usersService.GetUserByHandleAsync(handle));
    }

    [HttpDelete("{handle}")]
    public async Task<IActionResult> DeleteUserByHandleAsync(string handle)
    {
        await _usersService.DeleteUserByHandleAsync(handle);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllUsersAsync()
    {
        await _usersService.DeleteAllUsersAsync();

        return Ok();
    }
}