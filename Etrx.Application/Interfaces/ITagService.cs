using Etrx.Application.Dtos.Tags;
using Etrx.Domain.Models;

namespace Etrx.Application.Interfaces;

public interface ITagService
{
    Task<IList<GetTagResponseDto>> GetAllTagsAsync();
    Task UpdateTagComplexityAsync(string name, UpdateTagComplexityRequestDto dto);
}