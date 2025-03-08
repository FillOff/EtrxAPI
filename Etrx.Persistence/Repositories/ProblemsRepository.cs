using EFCore.BulkExtensions;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories
{
    public class ProblemsRepository : IProblemsRepository
    {
        private readonly EtrxDbContext _context;

        public ProblemsRepository(EtrxDbContext context)
        {
            _context = context;
        }

        public async Task<List<Problem>> Get()
        {
            var problems = await _context.Problems
                .AsNoTracking()
                .ToListAsync();

            return problems;
        }

        public async Task<List<Problem>> GetByContestId(int contestId)
        {
            var problems = await _context.Problems
                .AsNoTracking()
                .Where(p => p.ContestId == contestId)
                .ToListAsync();

            return problems;
        }

        public async Task<Problem?> GetByContestIdAndIndex(int contestId, string index)
        {
            var problem = await _context.Problems
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ContestId == contestId && p.Index == index);

            return problem;
        }

        public async Task<List<string>> GetAllTags()
        {
            var tags = await _context.Problems
                .AsNoTracking()
                .Where(problem => problem.Tags != null)
                .SelectMany(problem => problem.Tags!)
                .Distinct()
                .OrderBy(tag => tag)
                .ToListAsync();

            return tags;
        }

        public async Task<List<string>> GetAllIndexes()
        {
            var indexes = await _context.Problems
                .AsNoTracking()
                .Select(problem => problem.Index)
                .Distinct()
                .OrderBy(index => index)
                .ToListAsync();

            return indexes;
        }

        public async Task<List<string>> GetIndexesByContestId(int contestId)
        {
            var indexes = await _context.Problems
                .AsNoTracking()
                .Where(p => p.ContestId == contestId)
                .Select(p => p.Index)
                .ToListAsync();

            return indexes;
        }

        public async Task<int> Create(Problem problem)
        {
            await _context.Problems.AddAsync(problem);
            await _context.SaveChangesAsync();

            return problem.Id;
        }

        public async Task InsertOrUpdate(List<Problem> problems)
        {
            await _context.BulkInsertOrUpdateAsync(problems);
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
