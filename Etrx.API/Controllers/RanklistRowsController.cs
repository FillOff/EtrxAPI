using Etrx.Application.Interfaces;
using Etrx.Domain.Dtos.RanklistRows;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RanklistRowsController : ControllerBase
{
    private readonly IRanklistRowsService _ranklistRowsService;

    public RanklistRowsController(IRanklistRowsService ranklistRowsService)
    {
        _ranklistRowsService = ranklistRowsService;
    }

    [HttpGet("{contestId:int}")]
    public async Task<ActionResult<GetRanklistRowsResponseWithPropsDto>> GetRanklistRowsByContestIdWithSort(
        [FromRoute] int contestId,
        [FromQuery] GetRanklistRowsRequestDto dto)
    {
        return Ok(await _ranklistRowsService.GetRanklistRowsWithSortAsync(contestId, dto));
    }
}
