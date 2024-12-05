namespace Etrx.Domain.Interfaces.Services
{
    public interface IUpdateDataService
    {
        Task<(bool Success, string Error)> UpdateContests();
        Task<(bool Success, string Error)> UpdateProblems();
        Task<(bool Success, string Error)> UpdateUsers();
        Task<(bool Success, string Error)> UpdateSubmissions();
    }
}