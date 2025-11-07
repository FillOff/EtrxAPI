using Etrx.Domain.Dtos.Submissions;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;

namespace Etrx.Domain.Interfaces;

public interface ISubmissionsRepository : IGenericRepository<Submission, ulong>
{
    new Task<List<Submission>> GetAllAsync();
    Task<List<Submission>> GetByContestIdAsync(int contestId);
    Task<List<string>> GetUserParticipantTypesAsync(string handle);
    Task<List<GetGroupSubmissionsProtocolResponseDto>> GetGroupProtocolWithSortAsync(GroupProtocolQueryParameters parameters);
    Task<List<Submission>> GetByHandleAndContestIdAsync(HandleContestProtocolQueryParameters parameters);
}