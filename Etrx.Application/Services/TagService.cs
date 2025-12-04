using Etrx.Application.Dtos.Tags;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Domain.Models;

namespace Etrx.Application.Services;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;

    public TagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _unitOfWork.Tags.GetAllAsync();
    }

    public async Task<bool> UpdateTagComplexityAsync(string name, UpdateTagComplexityRequestDto dto)
    {
        var tag = await _unitOfWork.Tags.GetByNameAsync(name);
        if (tag == null) return false;

        tag.Complexity = dto.Complexity;

        _unitOfWork.Tags.Update(tag);
        await _unitOfWork.SaveAsync();

        return true;
    }
}