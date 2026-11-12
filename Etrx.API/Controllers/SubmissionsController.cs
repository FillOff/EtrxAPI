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
    public async Task<IActionResult> GetUsersProtocolAsync(
        [FromQuery] GetUsersProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetUsersProtocolAsync(dto));
    }

    [HttpGet("results")]
    public async Task<IActionResult> GetDivisionResultsAsync(
        [FromQuery] GetDivisionResultsRequestDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _submissionsService.GetDivisionResultsAsync(dto, cancellationToken));
    }

    [HttpGet("results-matrix")]
    public async Task<IActionResult> GetResultsMatrixAsync(
        [FromQuery] GetDivisionResultsRequestDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _submissionsService.GetResultsMatrixAsync(dto, cancellationToken));
    }

    [HttpGet("results-by-type")]
    public async Task<IActionResult> GetResultsByTypeAsync(
        [FromQuery] GetDivisionResultsRequestDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _submissionsService.GetResultsByTypeAsync(dto, cancellationToken));
    }

    [HttpGet("results-tasks/{handle}")]
    public async Task<IActionResult> GetResultsTasksAsync(
        [FromRoute] string handle,
        [FromQuery] GetDivisionResultsRequestDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _submissionsService.GetResultsTasksAsync(handle, dto, cancellationToken));
    }

    [HttpGet("results-summary")]
    public async Task<IActionResult> GetResultsSummaryAsync(
        [FromQuery] GetDivisionResultsRequestDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _submissionsService.GetResultsSummaryAsync(dto, cancellationToken));
    }

    [HttpGet("{handle}")]
    public async Task<IActionResult> GetUserContestProtocolAsync(
        [FromRoute] string handle,
        [FromQuery] GetUserContestProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetUserProtocolAsync(handle, dto));
    }

    [HttpGet("{handle}/{contestId:int}")]
    public async Task<IActionResult> GetUserContestProtocolAsync(
        [FromRoute] string handle,
        [FromRoute] int contestId,
        [FromQuery] GetUserContestProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetUserContestProtocolAsync(handle, contestId, dto));
    }
}
