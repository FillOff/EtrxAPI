using Etrx.Core.Interfaces.Repositories;
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

        public async Task<IEnumerable<Contest>> GetAllContests()
        {
            return await _contestsRepository.Get();
        }

        public async Task<int> CreateContest(Contest contest)
        {
            if (await _contestsRepository.GetById(contest.Id) == null)
            {
                return await _contestsRepository.Create(contest);
            }
            return -1;
        }
    }
}