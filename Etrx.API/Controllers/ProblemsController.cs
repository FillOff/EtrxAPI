using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Etrx.API.Controllers;

[ApiController]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[Route("api/problems")]
public class ProblemsController : ControllerBase
{
    private readonly IProblemsService _problemsService;
    private readonly IProblemProgressService _progressService;

    public ProblemsController(IProblemsService problemsService, IProblemProgressService progressService)
    {
        _problemsService = problemsService;
        _progressService = progressService;
    }

    [HttpGet("{contestId:int}")]
    public async Task<IActionResult> GetProblemsByContestIdAsync(
        [FromRoute] int contestId,
        [FromQuery] string lang = Languages.Ru)
    {
        return Ok(await _problemsService.GetProblemsByContestIdAsync(contestId, lang));
    }

    [HttpGet]
    public async Task<IActionResult> GetProblemsByPageWithSortAndFilterAsync(
        [FromQuery] GetSortProblemRequestDto dto, CancellationToken cancellationToken)
    {
        dto.CurrentUserId = CurrentUserId();
        return Ok(await _problemsService.GetProblemsByPageWithSortAndFilterAsync(dto, cancellationToken));
    }

    [HttpGet("filters")]
    public async Task<IActionResult> GetProblemFiltersAsync([FromQuery] GetSortProblemRequestDto dto, CancellationToken cancellationToken)
    {
        dto.CurrentUserId = CurrentUserId();
        return Ok(await _problemsService.GetProblemFiltersAsync(dto, cancellationToken));
    }

    private Guid? CurrentUserId() => User.Identity?.IsAuthenticated == true &&
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
