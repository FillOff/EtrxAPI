using AutoMapper;
using Etrx.Application.Dtos.Problems;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Profiles;

public class ProblemsProfile : Profile
{
    public ProblemsProfile()
    {
        string lang = "en";

        CreateMap<Problem, ProblemResponseDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                src.ProblemTranslations
                   .Where(pt => pt.LanguageCode == lang)
                   .Select(pt => pt.Name)
                   .FirstOrDefault() ?? "Unnamed Problem"))
            .AfterMap((src, dest, context) =>
            {
                if (context.Items.TryGetValue("lang", out var langObj) && langObj is string langValue)
                {
                    dest.Name = src.ProblemTranslations?
                        .FirstOrDefault(ct => ct.LanguageCode == langValue)?.Name ?? "Unnamed Problem";
                }
            })
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src =>
                src.Contest.StartTime))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(ProblemExpressions.DifficultyExpression));

        CreateMap<CodeforcesProblem, Problem>()
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SolvedCount, opt => opt.Ignore())
            .ForMember(dest => dest.ProblemTranslations, opt => opt.Ignore())
            .ForMember(dest => dest.GuidContestId, opt => opt.Ignore());

        CreateMap<CodeforcesProblem, ProblemTranslation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProblemId, opt => opt.Ignore())
            .ForMember(dest => dest.LanguageCode, opt => opt.Ignore());
    }
}
