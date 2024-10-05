using AutoMapper;
using Etrx.Domain.Models;
using Etrx.Persistence.Entities;

namespace Etrx.Persistence.DbProfiles
{
    public class ProblemsProfile : Profile
    {
        public ProblemsProfile()
        {
            CreateMap<Problem, ProblemEntity>();
            CreateMap<ProblemEntity, Problem>();
        }
    }
}
