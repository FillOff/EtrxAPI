using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Exceptions;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Authorize]
[Route("api/ladder/preferences")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class LadderPreferencesController : ControllerBase
{
    private readonly IProblemPreferencesService _preferencesService;
    private readonly IAuthSession _session;
    private Guid UserId => _session.UserId ?? throw new UnauthorizedException("Authentication is required.");

    public LadderPreferencesController(IProblemPreferencesService preferencesService, IAuthSession session)
    {
        _preferencesService = preferencesService;
        _session = session;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        return Ok(await _preferencesService.GetAsync(UserId, cancellationToken, ladder: true));
    }

    [HttpPut]
    [RequestSizeLimit(RequestLimits.PreferencesBodyBytes)]
    public async Task<IActionResult> SaveAsync(SaveProblemPreferencesRequestDto request, CancellationToken cancellationToken)
    {
        await _preferencesService.SaveAsync(UserId, _session.Current.Handle, request, cancellationToken, ladder: true);
        return NoContent();
    }
}
