using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Etrx.Application.Repositories;

namespace EtrxAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _tagRepository.GetAllAsync(CancellationToken.None));
        }

        [HttpPut("{id}/complexity")]
        public async Task<IActionResult> SetComplexity(Guid id, [FromBody] int complexity)
        {
            var tag = await _tagRepository.GetByIdAsync(id, CancellationToken.None);
            if (tag == null) return NotFound();

            tag.Complexity = complexity;
            await _tagRepository.UpdateAsync(tag, CancellationToken.None);

            return Ok();
        }
    }
}