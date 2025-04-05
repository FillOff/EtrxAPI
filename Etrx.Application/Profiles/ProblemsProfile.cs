using AutoMapper;
using Etrx.Core.Contracts.Problems;
using Etrx.Domain.Models;

namespace Etrx.Application.Profiles
{
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
        }
    }
}
