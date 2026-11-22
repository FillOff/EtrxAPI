using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Etrx.Application.Interfaces;
using Etrx.Application.Dtos.Submissions;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionsService _submissionsService;

    public SubmissionsController(ISubmissionsService submissionsService)
    {
        _submissionsService = submissionsService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetUsersProtocolAsync(
        [FromQuery] GetUsersProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetUsersProtocolAsync(dto));
    }

    [HttpGet("{handle}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserContestProtocolAsync(
        [FromRoute] string handle,
        [FromQuery] GetUserContestProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetUserProtocolAsync(handle, dto));
    }

    [HttpGet("{handle}/{contestId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserContestProtocolAsync(
        [FromRoute] string handle,
        [FromRoute] int contestId,
        [FromQuery] GetUserContestProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetUserContestProtocolAsync(handle, contestId, dto));
    }
}