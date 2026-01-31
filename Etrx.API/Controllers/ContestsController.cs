using Etrx.Application.Constants;
using Etrx.Application.Dtos.Contests;
using Etrx.Application.Interfaces;
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
    public async Task<IActionResult> GetContestByIdAsync(
        [FromRoute] int id,
        [FromQuery] string lang = Languages.Ru)
    {
        return Ok(await _contestsService.GetContestByIdAsync(id, lang));
    }

    [HttpGet]
    public async Task<IActionResult> GetContestsByPageWithSortAsync(
        [FromQuery] GetSortContestRequestDto dto)
    {
        return Ok(await _contestsService.GetContestsByPageWithSortAsync(dto));
    }
}