using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IProblemsService
    {
        IQueryable<Problem> GetAllProblems();
        IQueryable<Problem> GetProblemsByContestId(int contestId);
        Problem? GetProblemByContestIdAndIndex(int contestId, string index);
        Task<int> CreateProblem(Problem problem);
        Task<int> UpdateProblem(Problem problem);
        Task<int> DeleteProblem(int id);
        List<string?>? GetAllTags();
        (IQueryable<Problem> Problems, int PageCount) GetProblemsByPageWithSortAndFilterTags(int page, int pageSize, string? tags, string sortField, bool sortOrder);
        string[]? GetProblemsIndexesByContestId(int contestId);
    }
}