using Etrx.Application.Dtos.Submissions;
using Etrx.Domain.Models;
using Etrx.Application.Queries;

namespace Etrx.Application.Repositories;

public interface ISubmissionsRepository : IGenericRepository<Submission>
{
    new Task<List<Submission>> GetAllAsync();
    Task<List<Submission>> GetByContestIdAsync(int contestId);
    Task<List<string>> GetUserParticipantTypesAsync(string handle);
    Task<List<GetGroupSubmissionsProtocolResponseDto>> GetGroupProtocolWithSortAsync(GroupProtocolQueryParameters parameters);
    Task<List<Submission>> GetByHandleAndContestIdAsync(HandleContestProtocolQueryParameters parameters);
    Task<List<Submission>> GetBySubmissionIdsAsync(List<ulong> submissionIds);
}