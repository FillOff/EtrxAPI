using AutoMapper;
using Etrx.Core.Contracts.Contests;
using Etrx.Domain.Models;

namespace Etrx.Application.Profiles
{
    public class ContestsProfile : Profile
    {
        public ContestsProfile()
        {
            CreateMap<Contest, ContestResponseDto>();
        }
    }
}
