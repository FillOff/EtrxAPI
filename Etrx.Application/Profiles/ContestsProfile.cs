using AutoMapper;
using Etrx.Application.Dtos.Contests;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Profiles;

public class ContestsProfile : Profile
{
    public ContestsProfile()
    {
        string lang = "en";

        CreateMap<Contest, ContestResponseDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                src.ContestTranslations
                   .Where(ct => ct.LanguageCode == lang)
                   .Select(ct => ct.Name)
                   .FirstOrDefault() ?? "Unnamed Contest"))
            .AfterMap((src, dest, context) =>
            {
                if (context.Items.TryGetValue("lang", out var langObj) && langObj is string langValue)
                {
                    dest.Name = src.ContestTranslations?
                        .FirstOrDefault(ct => ct.LanguageCode == langValue)?.Name ?? "Unnamed Contest";
                }
            });

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
