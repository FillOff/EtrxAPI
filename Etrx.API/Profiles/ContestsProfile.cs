using AutoMapper;
using Etrx.API.Contracts.Contests;
using Etrx.Domain.Models;

namespace Etrx.API.Profiles
{
    public class ContestsProfile : Profile
    {
        public ContestsProfile()
        {
            CreateMap<Contest, ContestsResponse>();
        }
    }
}
