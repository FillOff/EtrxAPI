using AutoMapper;
using Etrx.Application.Dtos.Submissions;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Profiles;

public class SubmissionsProfile : Profile
{
    public SubmissionsProfile()
    {
        CreateMap<Submission, GetUserContestProtocolResponseDto>();

        CreateMap<CodeforcesSubmission, Submission>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SubmissionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Index, opt => opt.MapFrom(src => src.Problem.Index))
            .ForMember(dest => dest.ParticipantType, opt => opt.MapFrom(src => src.Author.ParticipantType))
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}
