using AutoMapper;
using Etrx.API.Contracts.Problems;
using Etrx.Domain.Models;

namespace Etrx.API.Profiles
{
    public class ProblemsProfile : Profile
    {
        public ProblemsProfile()
        {
            CreateMap<Problem, ProblemsResponse>();
        }
    }
}
