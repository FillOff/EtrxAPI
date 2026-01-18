using Etrx.Domain.Models.ParsingModels.Dl;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Interfaces;

public interface ICodeforcesService
{
    Task PostUserFromDlCodeforcesAsync(DlUser dlUser, CodeforcesUser cfUser);
    Task PostProblemsFromCodeforcesAsync(List<CodeforcesProblem> problems, List<CodeforcesProblemStatistics> problemStatistics, string languageCode);
    Task PostContestsFromCodeforcesAsync(List<CodeforcesContest> contests, bool gym, string languageCode);
    Task PostSubmissionsFromCodeforcesAsync(List<CodeforcesSubmission> submissions, string handle);
    Task PostRanklistRowsFromCodeforcesAsync(CodeforcesContestStanding contestStanding);
}