using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IProblemsService
    {
        IEnumerable<Problem> GetAllProblems();
        IEnumerable<Problem> GetProblemsByContestId(int contestId);
        Task<int> CreateProblem(Problem problem);
        Task<int> UpdateProblem(Problem problem);
        Task<int> DeleteProblem(int id);
    }
}