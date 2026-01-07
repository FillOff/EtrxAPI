using Etrx.Application.Interfaces;
using Etrx.Application.Dtos.Problems;
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
    public async Task<IActionResult> GetProblemsByContestId(
        [FromRoute] int contestId,
        [FromQuery] string lang = "ru")
    {
        return Ok(await _problemsService.GetProblemsByContestIdAsync(contestId, lang));
    }

    [HttpGet]
    public async Task<IActionResult> GetProblemsByPageWithSortAndFilter(
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