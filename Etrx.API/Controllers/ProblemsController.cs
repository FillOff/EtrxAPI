using Etrx.Application.Interfaces;
using Etrx.Core.Contracts.Problems;
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
    public async Task<ActionResult<List<ProblemResponseDto>>> GetProblemsByContestId(
        [FromRoute] int contestId,
        [FromQuery] string lang)
    {
        return Ok(await _problemsService.GetProblemsByContestIdAsync(contestId, lang));
    }

    [HttpGet]
    public ActionResult<ProblemWithPropsResponseDto> GetProblemsByPageWithSortAndFilterTags(
        [FromQuery] GetSortProblemRequestDto dto)
    {
        return Ok(_problemsService.GetProblemsByPageWithSortAndFilterTagsAsync(dto));
    }

    [HttpGet("tags")]
    public async Task<ActionResult<List<string>>> GetTagsList([FromQuery] int? minRating, [FromQuery] int? maxRating)
    {
        return Ok(await _problemsService.GetAllTagsAsync(minRating, maxRating));
    }

    [HttpGet("indexes")]
    public async Task<ActionResult<List<string>>> GetIndexesList()
    {
        return Ok(await _problemsService.GetAllIndexesAsync());
    }
}
