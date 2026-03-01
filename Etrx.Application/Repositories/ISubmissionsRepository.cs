using Etrx.Application.Dtos.Submissions;
using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface ISubmissionsRepository : IGenericRepository<Submission>
{
    new Task<List<Submission>> GetAllAsync();
    Task<List<Submission>> GetByContestIdAsync(int contestId);
    Task<List<string>> GetUserParticipantTypesAsync(string handle);
    Task<List<GetUsersProtocolsResponseDto>> GetUsersProtocolAsync(long unixFrom, long unixTo, int? contestId);
    Task<List<GetUserProtocolResponseDto>> GetUserProtocolAsync(string handle, long unixFrom, long unixTo);
    Task<List<Submission>> GetUserContestProtocolAsync(string handle, int contestId, long unixFrom, long unixTo);
    Task<List<Submission>> GetBySubmissionIdsAsync(List<ulong> submissionIds);
}