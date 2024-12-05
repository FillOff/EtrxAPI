using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Repositories
{
    public interface IProblemsRepository
    {
        Task<int> Create(Problem problem);
        Task<int> Delete(int id);
        IQueryable<Problem> Get();
        Task<int> Update(Problem problem);
        Problem? GetByContestIdAndIndex(int contestId, string index);
        Task InsertOrUpdateAsync(List<Problem> problems);
    }
}