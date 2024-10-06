using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Etrx.Core.Interfaces.Repositories;

namespace Etrx.Persistence.Repositories
{
    public class ContestsRepository : IContestsRepository
    {
        private readonly EtrxDbContext _context;

        public ContestsRepository(EtrxDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contest>> Get()
        {
            var contests = await _context.Contests
                .AsNoTracking()
                .ToListAsync();

            return contests;
        }

        public async Task<Contest?> GetById(int id)
        {
            var contest = await _context.Contests
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return contest;
        }

        public async Task<int> Create(Contest contest)
        {
            await _context.Contests.AddAsync(contest);
            await _context.SaveChangesAsync();

            return contest.Id;
        }

        public async Task<int> Update(Contest contest)
        {
            await _context.Contests
                .Where(c => c.Id == contest.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.Name, contest.Name)
                    .SetProperty(c => c.ContestId, contest.ContestId)
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

            return contest.Id;
        }

        public async Task<int> Delete(int id)
        {
            await _context.Contests
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}