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
    public async Task<IActionResult> PostAndUpdateProblemsFromCodeforces()
    {
        await _updateDataService.UpdateProblems();

        return Ok();
    }

    [HttpPost("contests")]
    public async Task<IActionResult> PostAndUpdateContestsFromCodeforces()
    {
        await _updateDataService.UpdateContests();

        return Ok();
    }

    [HttpPost("users")]
    public async Task<IActionResult> PostAndUpdateUsersFromDlCodeforces()
    {
        await _updateDataService.UpdateUsers();

        return Ok();
    }

    [HttpPost("submissions/{contestId:int}")]
    public async Task<IActionResult> PostAndUpdateSubmissionsFromCodeforcesByContestId([FromRoute] int contestId)
    {
        await _updateDataService.UpdateSubmissionsByContestId(contestId);

        return Ok();
    }

    [HttpPost("submissions")]
    public async Task<IActionResult> PostAndUpdateSubmissions()
    {
        await _updateDataService.UpdateSubmissions();

        return Ok();
    }

    [HttpPost("ranklistRows/{contestId:int}")]
    public async Task<IActionResult> PostAndUpdateRanklistRows([FromRoute] int contestId)
    {
        await _updateDataService.UpdateRanklistRowsByContestId(contestId);

        return Ok();
    }
}