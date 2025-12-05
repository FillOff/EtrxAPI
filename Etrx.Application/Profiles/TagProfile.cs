using AutoMapper;
using Etrx.Application.Dtos.Tags;
using Etrx.Domain.Models;

namespace Etrx.Application.Profiles;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<UpdateTagComplexityRequestDto, Tag>();

        CreateMap<Tag, GetTagResponseDto>();
    }
}