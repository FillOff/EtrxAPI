using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;

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
            var allContests = await _contestsRepository.Get();
            int pageCount = allContests.Count % pageSize == 0
                ? allContests.Count / pageSize
                : allContests.Count / pageSize + 1;

            string order = sortOrder == true ? "asc" : "desc";
            var contests = await _contestsRepository.GetByPageWithSort(
                page,
                pageSize,
                gym,
                sortField,
                order);

            return (contests, pageCount);
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