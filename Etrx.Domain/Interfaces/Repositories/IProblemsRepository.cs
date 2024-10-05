using Etrx.Domain.Enums;
using Etrx.Domain.Models;

namespace Etrx.Core.Interfaces.Repositories
{
    public interface IProblemsRepository
    {
        Task<int> Create(Problem problem);
        Task<int> Delete(int problemId);
        Task<IEnumerable<Problem>> Get();
        Task<Problem> GetById(int problemId);
        Task<int> Update(int problemId, int? contestId, string index, string name, string type, double? points, int? rating, string[] tags);
    }
}