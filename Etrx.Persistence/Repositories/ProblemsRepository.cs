using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories
{
    public class ProblemsRepository : GenericRepository<Problem, object>, IProblemsRepository
    {
        public ProblemsRepository(EtrxDbContext context)
            : base(context)
        { }

        public IQueryable<Problem> GetByContestId(int contestId)
        {
            return _dbSet
                .AsNoTracking()
                .Where(p => p.ContestId == contestId);
        }

        public IQueryable<string> GetAllTags()
        {
            return _dbSet
                .AsNoTracking()
                .Where(problem => problem.Tags != null)
                .SelectMany(problem => problem.Tags!)
                .Distinct()
                .OrderBy(tag => tag);
        }

        public IQueryable<string> GetAllIndexes()
        {
            return _dbSet
                .AsNoTracking()
                .Select(problem => problem.Index)
                .Distinct()
                .OrderBy(index => index);
        }

        public IQueryable<string> GetIndexesByContestId(int contestId)
        {
            return _dbSet
                .AsNoTracking()
                .Where(p => p.ContestId == contestId)
                .Select(p => p.Index);
        }
    }
}