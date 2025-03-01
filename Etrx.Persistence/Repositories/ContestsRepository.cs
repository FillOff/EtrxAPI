using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Persistence.Databases;
using EFCore.BulkExtensions;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories
{
    public class ContestsRepository : IContestsRepository
    {
        private readonly EtrxDbContext _context;

        public ContestsRepository(EtrxDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contest>> Get()
        {
            var contests = await _context.Contests
                .AsNoTracking()
                .ToListAsync();

            return contests;
        }

        public async Task<Contest?> GetById(int contestId)
        {
            var contest = await _context.Contests
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ContestId == contestId);

            return contest;
        }

        public async Task<List<Contest>> GetByPageWithSort(
            int page,
            int pageSize,
            bool? gym,
            string sortField,
            string order)
        {
            var contests = _context.Contests
                .AsNoTracking()
                .OrderBy($"{sortField} {order}")
                .Where(c => c.Phase != "BEFORE");

            if (gym != null)
            {
                contests = contests.Where(c => c.Gym == gym);
            }

            contests = contests
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return await contests.ToListAsync();
        }

        public async Task<int> Create(Contest contest)
        {
            await _context.Contests.AddAsync(contest);
            await _context.SaveChangesAsync();

            return contest.ContestId;
        }

        public async Task InsertOrUpdateAsync(List<Contest> contests)
        {
            await _context.BulkInsertOrUpdateAsync(contests);
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