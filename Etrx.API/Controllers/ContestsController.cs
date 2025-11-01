using Etrx.Application.Interfaces;
using Etrx.Domain.Dtos.Contests;
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
    public async Task<ActionResult<ContestResponseDto>> GetContestById(
        [FromRoute] int id,
        [FromQuery] string lang = "ru")
    {
        return Ok(await _contestsService.GetContestByIdAsync(id, lang));
    }

    [HttpGet]
    public async Task<ActionResult<ContestWithPropsResponseDto>> GetContestsByPageWithSort(
        [FromQuery] GetSortContestRequestDto dto)
    {
        return Ok(await _contestsService.GetContestsByPageWithSortAsync(dto));
    }
}