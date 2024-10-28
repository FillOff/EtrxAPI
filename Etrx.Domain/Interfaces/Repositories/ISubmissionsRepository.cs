using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Repositories
{
    public interface ISubmissionsRepository
    {
        Task<ulong> Create(Submission submission);
        Task<ulong> Delete(ulong id);
        IQueryable<Submission> Get();
        Submission? GetById(ulong id);
        Task<ulong> Update(Submission submission);
    }
}