using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Persistence.Databases;

namespace Etrx.Persistence.Repositories
{
    public class ContestsRepository : IContestsRepository
    {
        private readonly EtrxDbContext _context;

        public ContestsRepository(EtrxDbContext context)
        {
            _context = context;
        }

        public IQueryable<Contest> Get()
        {
            var contests = _context.Contests.AsNoTracking();

            return contests;
        }

        public Contest? GetById(int contestId)
        {
            return _context.Contests.AsNoTracking().FirstOrDefault(c => c.ContestId == contestId);
        }

        public async Task<int> Create(Contest contest)
        {
            await _context.Contests.AddAsync(contest);
            await _context.SaveChangesAsync();

            return contest.ContestId;
        }

        public async Task<int> Update(Contest contest)
        {
            await _context.Contests
                .Where(c => c.ContestId == contest.ContestId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.Name, contest.Name)
                    .SetProperty(c => c.Type, contest.Type)
                    .SetProperty(c => c.Phase, contest.Phase)
                    .SetProperty(c => c.Frozen, contest.Frozen)
                    .SetProperty(c => c.DurationSeconds, contest.DurationSeconds)
                    .SetProperty(c => c.StartTime, contest.StartTime)
                    .SetProperty(c => c.RelativeTimeSeconds, contest.RelativeTimeSeconds)
                    .SetProperty(c => c.PreparedBy, contest.PreparedBy)
                    .SetProperty(c => c.WebsiteUrl, contest.WebsiteUrl)
                    .SetProperty(c => c.Description, contest.Description)
                    .SetProperty(c => c.Difficulty, contest.Difficulty)
                    .SetProperty(c => c.Kind, contest.Kind)
                    .SetProperty(c => c.IcpcRegion, contest.IcpcRegion)
                    .SetProperty(c => c.Country, contest.Country)
                    .SetProperty(c => c.City, contest.City)
                    .SetProperty(c => c.Season, contest.Season)
                    .SetProperty(c => c.Gym, contest.Gym)
                );

            return contest.ContestId;
        }

        public async Task<int> Delete(int contestId)
        {
            await _context.Contests
                .Where(c => c.ContestId == contestId)
                .ExecuteDeleteAsync();

            return contestId;
        }
    }
}