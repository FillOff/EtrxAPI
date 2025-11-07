using Etrx.Domain.Models.ParsingModels.Dl;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Interfaces;

public interface ICodeforcesService
{
    Task PostUserFromDlCodeforces(DlUser dlUser, CodeforcesUser cfUser);
    Task PostProblemsFromCodeforces(List<CodeforcesProblem> problems, List<CodeforcesProblemStatistics> problemStatistics, string languageCode);
    Task PostContestsFromCodeforces(List<CodeforcesContest> contests, bool gym, string languageCode);
    Task PostSubmissionsFromCodeforces(List<CodeforcesSubmission> submissions, string handle);
    Task PostRanklistRowsFromCodeforces(CodeforcesContestStanding contestStanding);
}