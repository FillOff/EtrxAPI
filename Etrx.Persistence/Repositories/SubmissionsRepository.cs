using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Etrx.Persistence.Interfaces;

namespace Etrx.Persistence.Repositories;

public class SubmissionsRepository : GenericRepository<Submission, ulong>, ISubmissionsRepository
{
    public SubmissionsRepository(EtrxDbContext context)
        : base(context)
    { }

    public IQueryable<Submission> GetByContestId(int contestId)
    {
        return _dbSet
            .AsNoTracking()
            .Where(s => s.ContestId == contestId);
    }

    public IQueryable<string> GetUserParticipantTypes(string handle)
    {
        return _dbSet
            .AsNoTracking()
            .Where(s => s.Handle == handle)
            .Select(s => s.ParticipantType)
            .Distinct();
    }

    public IQueryable<Submission> GetByHandle(string handle)
    {
        return _dbSet
            .AsNoTracking()
            .Where(s => s.Handle == handle);
    }
}
