using Microsoft.AspNetCore.Mvc;
using Etrx.Application.Interfaces;
using Etrx.Domain.Dtos.Submissions;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionsService _submissionsService;

    public SubmissionsController(ISubmissionsService submissionsService)
    {
        _submissionsService = submissionsService;
    }

    [HttpGet]
    public async Task<ActionResult<GetGroupSubmissionsProtocolWithPropsResponseDto>> GetProtocol(
        [FromQuery] GetGroupSubmissionsProtocolRequestDto dto)
    {
        return Ok(await _submissionsService.GetGroupProtocolAsync(dto));
    }
}