using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CodeforcesController : ControllerBase
{
    private readonly IUpdateDataService _updateDataService;

    public CodeforcesController(
        IUpdateDataService updateDataService)
    {
        _updateDataService = updateDataService;
    }

    [HttpPost("problems")]
    public async Task<IActionResult> PostAndUpdateProblemsFromCodeforcesAsync()
    {
        await _updateDataService.UpdateProblemsAsync();

        return Ok();
    }

    [HttpPost("contests")]
    public async Task<IActionResult> PostAndUpdateContestsFromCodeforcesAsync()
    {
        await _updateDataService.UpdateContestsAsync();

        return Ok();
    }

    [HttpPost("users")]
    public async Task<IActionResult> PostAndUpdateUsersFromDlCodeforcesAsync()
    {
        await _updateDataService.UpdateUsersAsync();

        return Ok();
    }

    [HttpPost("submissions/{contestId:int}")]
    public async Task<IActionResult> PostAndUpdateSubmissionsFromCodeforcesByContestIdAsync([FromRoute] int contestId)
    {
        await _updateDataService.UpdateSubmissionsByContestIdAsync(contestId);

        return Ok();
    }

    [HttpPost("submissions")]
    public async Task<IActionResult> PostAndUpdateSubmissionsAsync()
    {
        await _updateDataService.UpdateSubmissionsAsync();

        return Ok();
    }

    [HttpPost("ranklistRows/{contestId:int}")]
    public async Task<IActionResult> PostAndUpdateRanklistRowsAsync([FromRoute] int contestId)
    {
        await _updateDataService.UpdateRanklistRowsByContestIdAsync(contestId);

        return Ok();
    }
}