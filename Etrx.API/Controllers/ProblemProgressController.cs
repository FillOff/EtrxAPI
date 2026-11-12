using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Dtos.Common;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController, Authorize, Route("api/problems/progress")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class ProblemProgressController(IProblemProgressService progressService) : ControllerBase
{
    [HttpPut("{contestId:int}/{index}/deferral")]
    public async Task<IActionResult> SetDeferred(int contestId, [StringLength(20, MinimumLength = 1)] string index,
        SetDeferredRequestDto request, CancellationToken ct)
    {
        if (contestId <= 0)
        {
            return BadRequest();
        }

        if (!string.Equals(User.Identity?.Name, request.ExpectedHandle, StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new MessageResponseDto("The account changed. The problem was not updated."));
        }

        if (!await progressService.SetDeferredAsync(UserId, contestId, index, request.Deferred, ct))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("synchronizations")]
    public async Task<IActionResult> Sync(CancellationToken ct) =>
        Ok(await progressService.SyncAsync(UserId, User.Identity!.Name!, ct));

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
