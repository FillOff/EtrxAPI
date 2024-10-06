using Etrx.Core.Interfaces.Repositories;
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

        public async Task<IEnumerable<Problem>> GetAllProblems()
        {
            return await _problemsRepository.Get();
        }

        public async Task<int> CreateProblem(Problem problem)
        {
            if (await _problemsRepository.GetByContestIdAndIndex(problem.ContestId, problem.Index) == null)
            {
                return await _problemsRepository.Create(problem);
            }
            return -1;
        }

        public async Task<int> UpdateProblem(Problem problem)
        {
            if (await _problemsRepository.GetByContestIdAndIndex(problem.ContestId, problem.Index) != null)
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
