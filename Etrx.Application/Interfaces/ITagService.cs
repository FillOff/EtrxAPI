using Etrx.Application.Dtos.Tags;

namespace Etrx.Application.Interfaces;

public interface ITagService
{
    Task<IList<TagsResponseDto>> GetTagsAsync();
    Task UpdateTagsAsync(UpdateTagsRequestDto dto);
}