using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IContestsService
    {
        Task<int> CreateContest(Contest contest);
        IQueryable<Contest> GetAllContests();
        Contest? GetContestById(int contestId);
        Task<int> UpdateContest(Contest contest);
        (IQueryable<Contest> Contests, int PageCount) GetContestsByPageWithSort(int page, int pageSize, bool? gym, string sortField = "contestid", bool sortOrder = true);
    }
}