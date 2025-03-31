using Etrx.Domain.Models;

namespace Etrx.Persistence.Interfaces;

public interface ISubmissionsRepository : IGenericRepository<Submission, ulong>
{
    IQueryable<Submission> GetByContestId(int contestId);
    IQueryable<string> GetUserParticipantTypes(string handle);
    IQueryable<Submission> GetByHandle(string handle);
}