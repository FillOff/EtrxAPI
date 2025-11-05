using Microsoft.AspNetCore.Mvc;
using Etrx.Application.Interfaces;
using Etrx.Domain.Dtos.Submissions;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionsService _submissionsService;

    public SubmissionsController(ISubmissionsService submissionsService)
    {
        _submissionsService = submissionsService;
    }

    [HttpGet]
    public async Task<ActionResult<GetGroupSubmissionsProtocolWithPropsResponseDto>> GetGroupProtocol(
        [FromQuery] GetGroupSubmissionsProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetGroupProtocolAsync(dto));
    }

    [HttpGet("{handle}/{contestId:int}")]
    public async Task<ActionResult<List<GetUserContestProtocolResponseDto>>> GetUserContestIdProtocolAsync(
        [FromRoute] string handle,
        [FromRoute] int contestId,
        [FromQuery] GetUserContestProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetUserContestProtocolAsync(handle, contestId, dto));
    }
}