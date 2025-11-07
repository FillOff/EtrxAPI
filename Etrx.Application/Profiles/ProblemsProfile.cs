using AutoMapper;
using Etrx.Domain.Dtos.Problems;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Profiles;

public class ProblemsProfile : Profile
{
    public ProblemsProfile()
    {
        CreateMap<Problem, ProblemResponseDto>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.ProblemTranslations.FirstOrDefault(pt => pt.LanguageCode == (string)context.Items["lang"])?.Name
                )
            );

        CreateMap<CodeforcesProblem, Problem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SolvedCount, opt => opt.Ignore())
            .ForMember(dest => dest.ProblemTranslations, opt => opt.Ignore());

        CreateMap<CodeforcesProblem, ProblemTranslation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProblemId, opt => opt.Ignore())
            .ForMember(dest => dest.LanguageCode, opt => opt.Ignore());
    }
}
