using Etrx.Application.Dtos.Tags;
using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<GetTagResponseDto>>> GetAll()
    {
        return Ok(await _tagService.GetAllTagsAsync());
    }

    [HttpPut("{name}")]
    public async Task<IActionResult> Update(string name, [FromBody] UpdateTagComplexityRequestDto dto)
    {
        try
        {
            await _tagService.UpdateTagComplexityAsync(name, dto);

            return Ok(new { Message = "Complexity updated" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}