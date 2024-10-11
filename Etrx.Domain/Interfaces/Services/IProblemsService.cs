using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IProblemsService
    {
        IQueryable<Problem> GetAllProblems();
        IQueryable<Problem> GetProblemsByContestId(int contestId);
        Task<int> CreateProblem(Problem problem);
        Task<int> UpdateProblem(Problem problem);
        Task<int> DeleteProblem(int id);
    }
}