using Etrx.Domain.Models.ParsingModels.Dl;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Domain.Interfaces.Services
{
    public interface ICodeforcesService
    {
        Task PostUsersFromDlCodeforces(List<DlUser> dlUsersList, List<CodeforcesUser> codeforcesUsersList);
        Task PostUserFromDlCodeforces(DlUser dlUser, CodeforcesUser cfUser);
        Task PostProblemsFromCodeforces(List<CodeforcesProblem> problems, List<CodeforcesProblemStatistics> problemStatistics);
        Task PostContestsFromCodeforces(List<CodeforcesContest> contests, bool gym);
        Task PostSubmissionsFromCodeforces(List<CodeforcesSubmission> submissions, string handle);
    }
}