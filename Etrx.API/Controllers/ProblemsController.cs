using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[Route("api/problems")]
public class ProblemsController : ControllerBase
{
    private readonly IProblemsService _problemsService;
    private readonly IAuthSession _authSession;

    public ProblemsController(IProblemsService problemsService, IAuthSession authSession)
    {
        _problemsService = problemsService;
        _authSession = authSession;
    }

    [HttpGet("{contestId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProblemsByContestIdAsync(
        [FromRoute] int contestId,
        [FromQuery] string lang = Languages.Ru)
    {
        return Ok(await _problemsService.GetProblemsByContestIdAsync(contestId, lang));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProblemsByPageWithSortAndFilterAsync(
        [FromQuery] GetSortProblemRequestDto dto, CancellationToken cancellationToken)
    {
        dto.CurrentUserId = _authSession.UserId;

        return Ok(await _problemsService.GetProblemsByPageWithSortAndFilterAsync(dto, cancellationToken));
    }

    [HttpGet("filters")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProblemFiltersAsync([FromQuery] GetSortProblemRequestDto dto, CancellationToken cancellationToken)
    {
        dto.CurrentUserId = _authSession.UserId;

        return Ok(await _problemsService.GetProblemFiltersAsync(dto, cancellationToken));
    }
}
