using Etrx.Application.Interfaces;
using Etrx.Application.Dtos.Users;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    public async Task<ActionResult<UsersWithPropsResponseDto>> GetUsersWithSort(
        [FromQuery] GetSortUserRequestDto dto)
    {
        return Ok(await _usersService.GetUsersWithSortAsync(dto));
    }

    [HttpGet("{handle}")]
    public async Task<ActionResult<UsersResponseDto>> GetUserByHandle(string handle)
    {
        return Ok(await _usersService.GetUserByHandleAsync(handle));
    }
}