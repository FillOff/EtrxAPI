namespace Etrx.Application.Interfaces;

public interface IUpdateDataService
{
    Task UpdateContests();
    Task UpdateProblems();
    Task UpdateUsers();
    Task UpdateSubmissions();
    Task UpdateSubmissionsByContestId(int contestId);
}