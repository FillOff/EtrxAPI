using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Application.Services
{
    public class ContestsService : IContestsService
    {
        private readonly IContestsRepository _contestsRepository;

        public ContestsService(IContestsRepository contestsRepository)
        {
            _contestsRepository = contestsRepository;
        }

        public async Task<List<Contest>> GetAllContestsAsync()
        {
            return await _contestsRepository.Get();
        }

        public async Task<Contest?> GetContestByIdAsync(int contestId)
        {
            return await _contestsRepository.GetById(contestId);
        }

        public async Task<(List<Contest> Contests, int PageCount)> GetContestsByPageWithSortAsync(
            int page,
            int pageSize,
            bool? gym,
            string sortField = "contestid",
            bool sortOrder = true)
        {
            string order = sortOrder == true ? "asc" : "desc";
            var contests = (await _contestsRepository.Get())
                .AsQueryable()
                .AsNoTracking()
                .OrderBy($"{sortField} {order}")
                .Where(c => c.Phase != "BEFORE");

            if (gym != null)
            {
                contests = contests.Where(c => c.Gym == gym);
            }

            int pageCount = contests.Count() % pageSize == 0
                ? contests.Count() / pageSize
                : contests.Count() / pageSize + 1;

            contests = contests
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return (contests.ToList(), pageCount);
        }

        public async Task<int> CreateContestAsync(Contest contest)
        {
            return await _contestsRepository.Create(contest);
        }

        public async Task<int> UpdateContestAsync(Contest contest)
        {
            return await _contestsRepository.Update(contest);
        }
    }
}