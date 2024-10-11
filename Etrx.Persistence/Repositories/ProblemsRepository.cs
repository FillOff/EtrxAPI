using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories
{
    public class ProblemsRepository : IProblemsRepository
    {
        private readonly EtrxDbContext _context;

        public ProblemsRepository(EtrxDbContext context)
        {
            _context = context;
        }

        public IQueryable<Problem> Get()
        {
            var problems = _context.Problems.AsNoTracking();

            return problems;
        }

        public Problem? GetByContestIdAndIndex(int contestId, string index)
        {
            return _context.Problems.AsNoTracking().FirstOrDefault(p => p.ContestId == contestId && p.Index == index);
        }

        public async Task<int> Create(Problem problem)
        {
            await _context.Problems.AddAsync(problem);
            await _context.SaveChangesAsync();

            return problem.Id;
        }

        public async Task<int> Update(Problem problem)
        {
            await _context.Problems
                .Where(p => p.Id == problem.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.ContestId, problem.ContestId)
                    .SetProperty(p => p.Index, problem.Index)
                    .SetProperty(p => p.Name, problem.Name)
                    .SetProperty(p => p.Type, problem.Type)
                    .SetProperty(p => p.Points, problem.Points)
                    .SetProperty(p => p.Rating, problem.Rating)
                    .SetProperty(p => p.SolvedCount, problem.SolvedCount)
                    .SetProperty(p => p.Tags, problem.Tags)
                );

            return problem.Id;
        }

        public async Task<int> Delete(int id)
        {
            await _context.Problems
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
