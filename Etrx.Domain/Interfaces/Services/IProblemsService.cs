using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IProblemsService
    {
        Task<int> CreateProblem(Problem problem);
        Task<int> DeleteProblem(int id);
        Task<IEnumerable<Problem>> GetAllProblems();
        Task<int> UpdateProblem(Problem problem);
    }
}