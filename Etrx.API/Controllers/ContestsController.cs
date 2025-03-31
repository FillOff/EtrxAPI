using Etrx.Application.Interfaces;
using Etrx.Core.Contracts.Contests;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContestsController : ControllerBase
{
    private readonly IContestsService _contestsService;

    public ContestsController(IContestsService contestsService)
    {
        _contestsService = contestsService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContestResponseDto>> GetContestById([FromRoute] int id)
    {
        return Ok(await _contestsService.GetContestByIdAsync(id));
    }

    [HttpGet]
    public ActionResult<ContestWithPropsResponseDto> GetContestsByPageWithSort(
        [FromQuery] GetSortContestRequestDto dto)
    {
        return Ok(_contestsService.GetContestsByPageWithSortAsync(dto));
    }
}