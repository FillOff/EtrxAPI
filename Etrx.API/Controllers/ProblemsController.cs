using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProblemsController : ControllerBase
{
    private readonly IProblemsService _problemsService;

    public ProblemsController(IProblemsService problemsService)
    {
        _problemsService = problemsService;
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
        [FromQuery] GetSortProblemRequestDto dto)
    {
        return Ok(await _problemsService.GetProblemsByPageWithSortAndFilterAsync(dto));
    }

    [HttpGet("filters")]
    public async Task<IActionResult> GetProblemFiltersAsync([FromQuery] GetSortProblemRequestDto dto)
    {
        return Ok(await _problemsService.GetProblemFiltersAsync(dto));
    }
}