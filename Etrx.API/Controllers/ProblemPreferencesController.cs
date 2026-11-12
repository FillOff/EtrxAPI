using System.Security.Claims;
using Etrx.Application.Dtos.Common;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Authorize]
[Route("api/problems/preferences")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class ProblemPreferencesController(IProblemPreferencesService preferencesService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        return Ok(await preferencesService.GetAsync(userId, cancellationToken));
    }

    [HttpPut]
    [RequestSizeLimit(32768)]
    public async Task<IActionResult> Save(SaveProblemPreferencesRequestDto request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        var result = await preferencesService.SaveAsync(userId, User.Identity?.Name, request, cancellationToken);
        return result switch
        {
            PreferenceSaveStatus.Success => NoContent(),
            PreferenceSaveStatus.AccountChanged => Conflict(new MessageResponseDto("The account changed. Filters were not saved.")),
            _ => BadRequest(new MessageResponseDto("Invalid filters."))
        };
    }
}
