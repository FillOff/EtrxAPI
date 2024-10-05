using AutoMapper;
using Etrx.Core.Interfaces.Repositories;
using Etrx.Domain.Enums;
using Etrx.Domain.Models;
using Etrx.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories
{
    public class ProblemsRepository : IProblemsRepository
    {
        private readonly EtrxDbContext _context;
        private readonly IMapper _mapper;

        public ProblemsRepository(EtrxDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Problem>> Get()
        {
            var problemEntities = await _context.Problems
                .AsNoTracking()
                .ToListAsync();

            var problems = _mapper.Map<List<Problem>>(problemEntities);

            return problems;
        }

        public async Task<Problem> GetById(int problemId)
        {
            var problemEntity = await _context.Problems.FirstOrDefaultAsync(p => p.ProblemId == problemId);

            var problem = _mapper.Map<Problem>(problemEntity);

            return problem;
        }

        public async Task<int> Create(Problem problem)
        {
            if (!_context.Problems.Any(p => p.ContestId == problem.ContestId && p.Index == problem.Index))
            { 
                var problemEntity = _mapper.Map<ProblemEntity>(problem);

                await _context.Problems.AddAsync(problemEntity);
                await _context.SaveChangesAsync();

                return problemEntity.ProblemId;
            }

            return 0;
        }

        public async Task<int> Update(int problemId, int? contestId, string index, string name, string type, double? points, int? rating, string[] tags)
        {
            await _context.Problems
                .Where(p => p.ProblemId == problemId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.ContestId, contestId)
                    .SetProperty(p => p.Index, index)
                    .SetProperty(p => p.Name, name)
                    .SetProperty(p => p.Type, type)
                    .SetProperty(p => p.Points, points)
                    .SetProperty(p => p.Rating, rating)
                    .SetProperty(p => p.Tags, tags)
                );

            return problemId;
        }

        public async Task<int> Delete(int problemId)
        {
            await _context.Problems
                .Where(p => p.ProblemId == problemId)
                .ExecuteDeleteAsync();

            return problemId;
        }
    }
}
