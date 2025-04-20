using Etrx.Domain.Models;

namespace Etrx.Persistence.Interfaces;

public interface ISubmissionsRepository : IGenericRepository<Submission, ulong>
{
    new IQueryable<Submission> GetAll();
    IQueryable<Submission> GetByContestId(int contestId);
    IQueryable<string> GetUserParticipantTypes(string handle);
    IQueryable<Submission> GetByHandle(string handle);
}