using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Models;
using Etrx.Domain.Interfaces.Services;

namespace Etrx.Application.Services
{
    public class ProblemsService : IProblemsService
    {
        private readonly IProblemsRepository _problemsRepository;

        public ProblemsService(IProblemsRepository problemsRepository)
        {
            _problemsRepository = problemsRepository;
        }

        public IEnumerable<Problem> GetAllProblems()
        {
            return _problemsRepository.Get().AsEnumerable();
        }

        public IEnumerable<Problem> GetProblemsByContestId(int contestId)
        {
            return _problemsRepository.Get().Where(p => p.ContestId == contestId).ToList();
        }

        public async Task<int> CreateProblem(Problem problem)
        {
            if (_problemsRepository.Get().FirstOrDefault(p => p.ContestId == problem.ContestId && p.Index == problem.Index) == null)
            {
                return await _problemsRepository.Create(problem);
            }
            return -1;
        }

        public async Task<int> UpdateProblem(Problem problem)
        {
            if (_problemsRepository.Get().FirstOrDefault(p => p.ContestId == problem.ContestId && p.Index == problem.Index) != null)
            {
                return await _problemsRepository.Update(problem);
            }
            return -1;
        }

        public async Task<int> DeleteProblem(int id)
        {
            return await _problemsRepository.Delete(id);
        }
    }
}
