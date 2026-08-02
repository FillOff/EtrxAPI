namespace Etrx.Application.Interfaces;

public interface IUpdateDataService
{
    Task UpdateContestsAsync();
    Task UpdateProblemsAsync();
    Task UpdateUsersAsync();
    Task UpdateSubmissionsAsync();
    Task UpdateSubmissionsByContestIdAsync(int contestId);
    Task UpdateRanklistRowsByContestIdAsync(int contestId);
    Task UpdateIoiContestsAsync();
    Task UpdateIoiRanklistRowsByContestIdAsync(int contestId);
}
