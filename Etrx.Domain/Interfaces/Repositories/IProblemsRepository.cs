using Etrx.Domain.Models;

namespace Etrx.Core.Interfaces.Repositories
{
    public interface IProblemsRepository
    {
        Task<int> Create(Problem problem);
        Task<int> Delete(int id);
        Task<IEnumerable<Problem>> Get();
        Task<Problem?> GetByContestIdAndIndex(int contestId, string index);
        Task<int> Update(Problem problem);
    }
}