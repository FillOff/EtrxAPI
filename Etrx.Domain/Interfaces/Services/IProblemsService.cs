using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IProblemsService
    {
        Task<int> CreateProblem(Problem problem);
        Task<int> DeleteProblem(int problemId);
        Task<IEnumerable<Problem>> GetAllProblems();
        Task<Problem> getProblemById(int problemId);
        Task<int> UpdateProblem(int problemId, int contestId, string index, string name, string type, double? points, int? rating, int solvedCount, string[] tags);
    }
}