using Etrx.API.Contracts.Submissions;
using Etrx.Domain.Interfaces.Services;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionsService _submissionsService;
        private readonly IUsersService _usersService;

        public SubmissionsController(ISubmissionsService submissionsService, 
                                     IUsersService usersService)
        {
            _submissionsService = submissionsService;
            _usersService = usersService;
        }

        [HttpGet("GetSubmissionsByContestIdWithSort")]
        public ActionResult<IEnumerable<SubmissionsWithProblemsResponse>> GetSubmissionsByContestIdWithSort(
            [FromQuery] int contestId,
            [FromQuery] string sortField = "solvedCount",
            [FromQuery] bool sortOrder = true)
        {
            if (string.IsNullOrEmpty(sortField) ||
                !typeof(SubmissionsResponse).GetProperties().Any(p => p.Name.Equals(sortField, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {sortField}");
            }
            string order = sortOrder == true ? "asc" : "desc";

            var submissions = _submissionsService.GetSubmissionsByContestId(contestId);

            var handles = submissions
                .Select(s => s.Handle)
                .Distinct()
                .ToArray();
            var users = handles.Select(_usersService.GetUserByHandle);
            
            List<SubmissionsResponse> submissionsResponses = [];

            var indexes = submissions
                .Select(s => s.Index)
                .Distinct()
                .ToArray();

            foreach (var handle in handles)
            {
                var userSubmissions = submissions.Where(s => s.Handle == handle);

                var (solvedCount, tries) = _submissionsService.GetTriesAndSolvedCountByHandle(handle, userSubmissions, indexes);

                var user = users.FirstOrDefault(u => u!.Handle.Equals(handle, StringComparison.CurrentCultureIgnoreCase))!;
                var submissionResponse = new SubmissionsResponse(handle, user.FirstName, user.LastName, user.City, user.Organization,
                                                                 user.Grade, solvedCount, userSubmissions.FirstOrDefault(s => s.Handle == handle)!.ParticipantType, tries);

                submissionsResponses.Add(submissionResponse);
            }

            submissionsResponses = submissionsResponses
                .AsQueryable()
                .OrderBy($"{sortField} {order}")
                .ToList();

            var response = new SubmissionsWithProblemsResponse(
                Submissions: submissionsResponses,
                ProblemIndexes: indexes,
                Properties: typeof(SubmissionsResponse).GetProperties().Select(p => p.Name).ToArray());

            return Ok(response);
        }
    }
}