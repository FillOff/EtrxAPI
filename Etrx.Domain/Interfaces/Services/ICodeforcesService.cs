using Etrx.Core.Parsing_models;
using Etrx.Domain.Parsing_models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface ICodeforcesService
    {
        Task PostUsersFromDlCodeforces(List<DlUser> dlUsersList, List<CodeforcesUser> codeforcesUsersList);
        Task PostProblemsFromCodeforces(List<CodeforcesProblem> problems, List<CodeforcesProblemStatistics> problemStatistics);
        Task PostContestsFromCodeforces(List<CodeforcesContest> contests, bool gym);
        Task PostSubmissionsFromCodeforces(List<CodeforcesSubmission> submissions, string handle);
    }
}