using AutoMapper;
using Etrx.Domain.Dtos.Contests;
using Etrx.Domain.Models;

namespace Etrx.Application.Profiles
{
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
        }
    }
}
