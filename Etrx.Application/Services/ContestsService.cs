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
            if (_contestsRepository.GetById(contest.ContestId) == null)
            {
                return await _contestsRepository.Create(contest);
            }
            return -1;
        }
    }
}