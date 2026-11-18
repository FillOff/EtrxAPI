using Etrx.Application.Dtos.Problems;
using Etrx.Application.Exceptions;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController, Authorize, Route("api/problems/progress")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class ProblemProgressController : ControllerBase
{
    private readonly IProblemProgressService _progressService;
    private readonly IAuthSession _session;
    private Guid UserId => _session.UserId ?? throw new UnauthorizedException("Authentication is required.");

    public ProblemProgressController(IProblemProgressService progressService, IAuthSession session)
    {
        _progressService = progressService;
        _session = session;
    }

    [HttpPut("{contestId:int}/{index}/deferral")]
    public async Task<IActionResult> SetDeferredAsync(int contestId, string index,
        SetDeferredRequestDto request, CancellationToken cancellationToken)
    {
        await _progressService.SetDeferredAsync(UserId, _session.Current.Handle, contestId, index, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("synchronizations")]
    public async Task<IActionResult> SyncAsync(CancellationToken cancellationToken)
    {
        return Ok(await _progressService.SyncAsync(UserId, _session.Current.Handle!, cancellationToken));
    }
}
