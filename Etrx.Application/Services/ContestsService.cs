using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
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

        public IQueryable<Contest> GetAllContests()
        {
            return _contestsRepository.Get();
        }

        public Contest? GetContestById(int contestId)
        {
            return _contestsRepository.GetById(contestId);
        }

        public async Task<int> CreateContest(Contest contest)
        {
            return await _contestsRepository.Create(contest);
        }

        public async Task<int> UpdateContest(Contest contest)
        {
            return await _contestsRepository.Update(contest);
        }

        public (IQueryable<Contest> Contests, int PageCount) GetContestsByPageWithSort(int page, int pageSize, bool? gym, string sortField = "contestid", bool sortOrder = true)
        {
            var contests = gym != null
                                ? _contestsRepository.Get().Where(c => c.Gym == gym)
                                : _contestsRepository.Get();

            int pageCount = contests.Count() % pageSize == 0
                ? contests.Count() / pageSize
                : contests.Count() / pageSize + 1;

            string order = sortOrder == true ? "asc" : "desc";

            contests = contests
                .OrderBy($"{sortField} {order}")
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return (contests, pageCount);
        }
    }
}