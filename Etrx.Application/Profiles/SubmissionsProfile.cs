using AutoMapper;
using Etrx.Domain.Contracts.Submissions;
using Etrx.Domain.Models;

namespace Etrx.Application.Profiles;

public class SubmissionsProfile : Profile
{
    public SubmissionsProfile()
    {
        CreateMap<Submission, GetSubmissionsProtocolResponseDto>()
            .ForMember(
                dest => dest.FirstName,
                opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(
                dest => dest.LastName,
                opt => opt.MapFrom(src => src.User.LastName));

        CreateMap<Submission, GetGroupSubmissionsProtocolResponseDto>()
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src => src.User.LastName + " " + src.User.FirstName));
    }
}
