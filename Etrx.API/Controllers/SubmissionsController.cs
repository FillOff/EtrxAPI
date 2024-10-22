using Etrx.API.Contracts.Submissions;
using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionsService _submissionsService;
        private readonly IUsersService _usersService;

        public SubmissionsController(ISubmissionsService submissionsService, IUsersService usersService)
        {
            _submissionsService = submissionsService;
            _usersService = usersService;
        }

        [HttpGet("GetSubmissionsByContestId")]
        public ActionResult<IEnumerable<SubmissionsWithProblemsResponse>> GetSubmissionsByContestId(int contestId)
        {

            var contestSubmissions = _submissionsService.GetAllSubmissions().Where(s => s.ContestId == contestId);
            
            var handles = contestSubmissions.Select(s => s.Handle).Distinct().ToArray();
            var users = handles.Select(_usersService.GetUserByHandle);
            
            SubmissionsResponse[] submissionsResponses = new SubmissionsResponse[handles.Length];

            var indexes = contestSubmissions.Select(s => s.Index).Distinct().ToArray();
            
            int j = 0;
            foreach (var handle in handles)
            {
                var userSubmissions = contestSubmissions.Where(s => s.Handle == handle);

                int solvedCount = 0;
                int[] tries = new int[indexes.Length];
                int i = 0;
                foreach (var index in indexes)
                {
                    var indexSubmissions = userSubmissions.Where(s => s.Index == index);

                    int tryCount = indexSubmissions.Count();

                    if (indexSubmissions.Any(s => s.Verdict == "Ok"))
                    {
                        solvedCount++;
                        tries[i] = tryCount;
                    }
                    else
                    {
                        tries[i] = -tryCount;
                    }
                    i++;
                }
                var user = users.FirstOrDefault(u => u!.Handle.Equals(handle, StringComparison.CurrentCultureIgnoreCase))!;
                SubmissionsResponse sub = new SubmissionsResponse(handle, user.FirstName, user.LastName, user.City, user.Organization,
                                                                    user.Grade, solvedCount, userSubmissions.FirstOrDefault(s => s.Handle == handle)!.ParticipantType, tries);
                submissionsResponses[j] = sub;
                j++;
            }

            var response = new SubmissionsWithProblemsResponse(submissionsResponses, indexes);
            return Ok(response);
        }
    }
}