using Etrx.Core.Models;

namespace Etrx.Domain.Interfaces.Repositories
{
    public interface ISubmissionsRepository
    {
        Task<ulong> Create(Submission submission);
        Task<ulong> Delete(ulong id);
        IQueryable<Submission> Get();
        Submission? GetByContestAndIndex(int? contestId, string index);
        Task<ulong> Update(Submission submission);
    }
}