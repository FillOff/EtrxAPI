using Etrx.Application.Dtos.Tags;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Route("/api/tags")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTagsAsync()
    {
        var response = await _tagService.GetTagsAsync();

        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTagsAsync([FromBody] UpdateTagsRequestDto dto)
    {
        await _tagService.UpdateTagsAsync(dto);
        
        return Ok();
    }
}
