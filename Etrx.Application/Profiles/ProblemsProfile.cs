using AutoMapper;
using Etrx.Core.Contracts.Problems;
using Etrx.Domain.Models;

namespace Etrx.Application.Profiles
{
    public class ProblemsProfile : Profile
    {
        public ProblemsProfile()
        {
            CreateMap<Problem, ProblemResponseDto>();
        }
    }
}
