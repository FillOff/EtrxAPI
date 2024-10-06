using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IContestsService
    {
        Task<int> CreateContest(Contest contest);
        Task<IEnumerable<Contest>> GetAllContests();
    }
}