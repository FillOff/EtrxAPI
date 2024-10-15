using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface ISubmissionsService
    {
        Task<ulong> CreateSubmission(Submission submission);
        Task<ulong> DeleteSubmission(ulong id);
        IQueryable<Submission> GetAllSubmissions();
        Task<ulong> UpdateSubmission(Submission submission);
    }
}