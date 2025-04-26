using Microsoft.AspNetCore.Mvc;
using Etrx.Application.Interfaces;
using Etrx.Core.Contracts.Submissions;
using Etrx.Domain.Contracts.Submissions;

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

    [HttpGet("{contestId:int}")]
    public async Task<ActionResult<SubmissionsWithProblemIndexesResponseDto>> GetSubmissionsByContestIdWithSort(
        [FromRoute] int contestId,
        [FromQuery] GetSubmissionRequestDto dto)
    {
        return Ok(await _submissionsService.GetSubmissionsWithSortAsync(contestId, dto));
    }

    [HttpGet]
    public async Task<ActionResult<GetSubmissionsWithPropsProtocolResponseDto>> GetProtocol(
        [FromQuery] GetGroupSubmissionsProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetGroupProtocolAsync(dto));
    }
}