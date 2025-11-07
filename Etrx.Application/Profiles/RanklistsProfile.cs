using AutoMapper;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Profiles;

public class RanklistsProfile : Profile
{
    public RanklistsProfile()
    {
        CreateMap<CodeforcesRanklistRow, RanklistRow>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ContestId, opt => opt.Ignore())
            .ForMember(dest => dest.Handle, opt => opt.MapFrom(src => src.Party.Members[0].Handle))
            .ForMember(dest => dest.ParticipantType, opt => opt.MapFrom(src => src.Party.ParticipantType))
            .ForMember(dest => dest.ProblemResults, opt => opt.Ignore());

        CreateMap<CodeforcesProblemResult, ProblemResult>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RanklistRowId, opt => opt.Ignore())
            .ForMember(dest => dest.Index, opt => opt.Ignore());
    }
}