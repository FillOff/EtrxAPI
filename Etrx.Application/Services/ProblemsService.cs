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

        public IQueryable<Problem> GetAllProblems()
        {
            return _problemsRepository.Get();
        }

        public Problem? GetProblemByContestIdAndIndex(int contestId, string index)
        {
            return _problemsRepository.GetByContestIdAndIndex(contestId, index);
        }

        public IQueryable<Problem> GetProblemsByContestId(int contestId)
        {
            return _problemsRepository.Get().Where(p => p.ContestId == contestId);
        }

        public async Task<int> CreateProblem(Problem problem)
        {
            if (_problemsRepository.GetByContestIdAndIndex(problem.ContestId, problem.Index) == null)
            {
                return await _problemsRepository.Create(problem);
            }
            return -1;
        }

        public async Task<int> UpdateProblem(Problem problem)
        {
            if (_problemsRepository.GetByContestIdAndIndex(problem.ContestId, problem.Index) != null)
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
