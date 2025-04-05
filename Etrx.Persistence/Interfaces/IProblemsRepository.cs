using Etrx.Domain.Models;

namespace Etrx.Persistence.Interfaces;

public interface IProblemsRepository : IGenericRepository<Problem, object>
{
    new IQueryable<Problem> GetAll();
    Task<Problem?> GetByKey(int contestId, string index);
    IQueryable<Problem> GetByContestId(int contestId);
    IQueryable<string> GetAllTags();
    IQueryable<string> GetAllIndexes();
    IQueryable<string> GetIndexesByContestId(int contestId);
}