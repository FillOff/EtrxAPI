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
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _tagService.GetAllTagsAsync());
    }

    [HttpPatch("{name}")]
    public async Task<IActionResult> UpdateComplexityAsync(
        [FromRoute] string name,
        [FromBody] UpdateTagComplexityRequestDto dto)
    {
        var result = await _tagService.UpdateTagComplexityAsync(name, dto);

        if (!result) return NotFound($"Тег '{name}' не найден.");

        return Ok(new { Message = "Сложность обновлена" });
    }
}