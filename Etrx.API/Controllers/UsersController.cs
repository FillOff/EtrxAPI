using Etrx.Application.Interfaces;
using Etrx.Application.Dtos.Users;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public async Task<IActionResult> GetUsersWithSortAsync(
        [FromQuery] GetSortUserRequestDto dto)
    {
        return Ok(await _usersService.GetUsersWithSortAsync(dto));
    }

    [HttpGet("{handle}")]
    public async Task<IActionResult> GetUserByHandleAsync(string handle)
    {
        return Ok(await _usersService.GetUserByHandleAsync(handle));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllUsersAsync()
    {
        await _usersService.DeleteAllUsersAsync();

        return Ok();
    }
}