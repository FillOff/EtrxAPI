using AutoMapper;
using Etrx.Application.Dtos.Users;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;
using Etrx.Domain.Models.ParsingModels.Dl;

namespace Etrx.Application.Profiles;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        CreateMap<User, UsersResponseDto>();

        CreateMap<CodeforcesUser, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FirstName, opt => opt.Ignore())
            .ForMember(dest => dest.LastName, opt => opt.Ignore())
            .ForMember(dest => dest.Grade, opt => opt.Ignore())
            .ForMember(dest => dest.Organization, opt => opt.Ignore())
            .ForMember(dest => dest.City, opt => opt.Ignore());

        CreateMap<DlUser, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Handle, opt => opt.Ignore());
    }
}