using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Repositories
{
    public interface ISubmissionsRepository
    {
        Task<List<Submission>> Get();
        Task<Submission?> GetById(ulong id);
        Task<List<Submission>> GetByContestId(int contestId);
        Task<List<string>> GetUserParticipantTypes(string handle);
        Task<List<Submission>> GetByHandle(string handle);
        Task<ulong> Create(Submission submission);
        Task InsertOrUpdateAsync(List<Submission> submissions);
        Task<ulong> Update(Submission submission);
        Task<ulong> Delete(ulong id);
    }
}