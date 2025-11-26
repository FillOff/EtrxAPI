using AutoMapper;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Services;
using Etrx.Domain.Helpers;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Profiles;

public class ProblemsProfile : Profile
{
    public ProblemsProfile()
    {

        CreateMap<Problem, ProblemResponseDto>()
           .ForMember(dest => dest.Name,
               opt => opt.MapFrom(src =>
                   src.ProblemTranslations
                       .FirstOrDefault(t => t.LanguageCode == "ru") != null
                       ? src.ProblemTranslations.FirstOrDefault(t => t.LanguageCode == "ru")!.Name
                       : string.Empty
               ))
           .ForMember(dest => dest.Division,
               opt => opt.MapFrom(src => src.Division /*DivisionHelper.GetDivisionName(src.Rating)*/));

        CreateMap<CodeforcesProblem, Problem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SolvedCount, opt => opt.Ignore())
            .ForMember(dest => dest.ProblemTranslations, opt => opt.Ignore())
            .ForMember(dest => dest.GuidContestId, opt =>

        CreateMap<CodeforcesProblem, ProblemTranslation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProblemId, opt => opt.Ignore())
            .ForMember(dest => dest.LanguageCode, opt => opt.Ignore()));
    }
}
