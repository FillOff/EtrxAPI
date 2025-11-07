using AutoMapper;
using Etrx.Domain.Dtos.Contests;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Profiles;

public class ContestsProfile : Profile
{
    public ContestsProfile()
    {
        CreateMap<Contest, ContestResponseDto>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.ContestTranslations.FirstOrDefault(сt => сt.LanguageCode == (string)context.Items["lang"])?.Name
                )
            );

        CreateMap<CodeforcesContest, Contest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Gym, opt => opt.Ignore())
            .ForMember(dest => dest.IsContestLoaded, opt => opt.Ignore())
            .ForMember(dest => dest.ContestTranslations, opt => opt.Ignore());

        CreateMap<CodeforcesContest, ContestTranslation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ContestId, opt => opt.Ignore())
            .ForMember(dest => dest.LanguageCode, opt => opt.Ignore());
    }
}
