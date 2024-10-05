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

        public async Task<Problem> getProblemById(int problemId)
        {
            return await _problemsRepository.GetById(problemId);
        }

        public async Task<int> CreateProblem(Problem problem)
        {
            return await _problemsRepository.Create(problem);
        }

        public async Task<int> UpdateProblem(int problemId, int contestId, string index, string name, string type, double? points, int? rating, int solvedCount, string[] tags)
        {
            return await _problemsRepository.Update(problemId, contestId, index, name, type, points, rating, solvedCount, tags);
        }

        public async Task<int> DeleteProblem(int problemId)
        {
            return await _problemsRepository.Delete(problemId);
        }
    }
}
