using Etrx.Application.Interfaces;
using Etrx.Domain.Dtos.Problems;
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
        [FromQuery] string lang = "ru")
    {
        return Ok(await _problemsService.GetProblemsByContestIdAsync(contestId, lang));
    }

    [HttpGet]
    public async Task<ActionResult<ProblemWithPropsResponseDto>> GetProblemsByPageWithSortAndFilterTags(
        [FromQuery] GetSortProblemRequestDto dto)
    {
        return Ok(await _problemsService.GetProblemsByPageWithSortAndFilterAsync(dto));
    }

    [HttpGet("tags")]
    public async Task<ActionResult<List<string>>> GetTagsList([FromQuery] GetAllTagsRequestDto dto)
    {
        return Ok(await _problemsService.GetAllTagsAsync(dto));
    }

    [HttpGet("indexes")]
    public async Task<ActionResult<List<string>>> GetIndexesList()
    {
        return Ok(await _problemsService.GetAllIndexesAsync());
    }
}
