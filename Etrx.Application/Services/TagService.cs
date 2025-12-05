using AutoMapper;
using Etrx.Application.Dtos.Tags;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories.UnitOfWork;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Etrx.Application.Services;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TagService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IList<GetTagResponseDto>> GetAllTagsAsync()
    {
        var tags = await _unitOfWork.Tags.GetAllAsync();
        return _mapper.Map<IList<GetTagResponseDto>>(tags);
    }

    public async Task UpdateTagComplexityAsync(string name, UpdateTagComplexityRequestDto dto)
    {
        var tag = await _unitOfWork.Tags.GetByNameAsync(name);

        if (tag == null)
        {
            throw new KeyNotFoundException($"Tag with name '{name}' not found.");
        }

        _mapper.Map(dto, tag);

        _unitOfWork.Tags.Update(tag);
        await _unitOfWork.SaveAsync();
    }
}