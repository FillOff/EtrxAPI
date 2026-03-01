using AutoMapper;
using Etrx.Application.Dtos.Tags;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories.UnitOfWork;

namespace Etrx.Application.Services;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TagService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IList<TagsResponseDto>> GetTagsAsync()
    {
        var tags = await _unitOfWork.Tags.GetAllAsync();
        var result = _mapper.Map<IList<TagsResponseDto>>(tags);

        return result;
    }

    public async Task UpdateTagsAsync(UpdateTagsRequestDto dto)
    {
        var tags = await _unitOfWork.Tags.GetAllAsync();

        foreach (var tag in tags)
        {
            tag.Priority = dto.Tags.First(d => d.Id == tag.Id).Priority;
            _unitOfWork.Tags.Update(tag);
        }

        await _unitOfWork.SaveAsync();
    }
}
