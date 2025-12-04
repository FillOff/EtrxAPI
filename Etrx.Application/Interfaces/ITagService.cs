using Etrx.Application.Dtos.Tags;
using Etrx.Domain.Models;

namespace Etrx.Application.Interfaces;

public interface ITagService
{
    Task<List<Tag>> GetAllTagsAsync();
    Task<bool> UpdateTagComplexityAsync(string name, UpdateTagComplexityRequestDto dto);
}