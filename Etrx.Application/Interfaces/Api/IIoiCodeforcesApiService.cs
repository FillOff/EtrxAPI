using Etrx.Domain.Models.Parsing_models.IoiCodeforces;

namespace Etrx.Application.Interfaces.Api;

public interface IIoiCodeforcesApiService
{
    Task<List<IoiCodeforcesContest>> GetContestsAsync();
    Task<IoiCodeforcesStandings> GetRanklistRowsByContestIdAsync(int contestId);
}