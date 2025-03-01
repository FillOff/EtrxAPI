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
        private readonly IProblemsService _problemsService;

        public SubmissionsController(ISubmissionsService submissionsService, 
                                     IUsersService usersService,
                                     IProblemsService problemsService)
        {
            _submissionsService = submissionsService;
            _usersService = usersService;
            _problemsService = problemsService;
        }

        [HttpGet("{contestId:int}")]
        public async Task<ActionResult<List<SubmissionsWithProblemsResponse>>> GetSubmissionsByContestIdWithSort(
            [FromRoute] int contestId,
            [FromQuery] string sortField = "solvedCount",
            [FromQuery] bool sortOrder = true,
            [FromQuery] bool allIndexes = true,
            [FromQuery] string filterByParticipantType = "ALL")
        {
            if (string.IsNullOrEmpty(sortField) ||
                !typeof(SubmissionsResponse).GetProperties().Any(p => p.Name.Equals(sortField, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {sortField}");
            }

            string order = sortOrder == true ? "asc" : "desc";

            var submissions = await _submissionsService.GetSubmissionsByContestIdAsync(contestId);

            var handles = submissions
                .Select(s => s.Handle)
                .Distinct()
                .ToArray();

            var tasks= handles.Select(async h => await _usersService.GetUserByHandleAsync(h)).ToList();
            var users = (await Task.WhenAll(tasks)).ToList();

            List<SubmissionsResponse> submissionsResponses = [];

            List<string>? indexes;
            if (allIndexes)
            {
                indexes = await _problemsService
                    .GetProblemsIndexesByContestIdAsync(contestId);
            }
            else
            {
                indexes = submissions
                    .Select(s => s.Index)
                    .Distinct()
                    .ToList();
            }

            foreach (var handle in handles)
            {
                var userSubmissions = submissions.Where(s => s.Handle == handle).ToList();
                List<string> types = await _submissionsService.GetUserParticipantTypesAsync(handle);
                foreach (var type in types)
                {
                    var typeSubmissions = userSubmissions.Where(s => s.ParticipantType == type).ToList();

                    var (solvedCount, tries) = _submissionsService.GetTriesAndSolvedCountByHandleAsync(typeSubmissions, indexes);
                    var user = users.FirstOrDefault(u => u!.Handle.Equals(handle, StringComparison.CurrentCultureIgnoreCase))!;
                    
                    var submissionResponse = new SubmissionsResponse(handle, user.FirstName, user.LastName, user.City, user.Organization,
                        user.Grade, solvedCount, type, tries);

                    submissionsResponses.Add(submissionResponse);
                }
            }

            if (filterByParticipantType != "ALL")
            {
                submissionsResponses = submissionsResponses
                    .Where(s => s.ParticipantType == filterByParticipantType)
                    .ToList();
            }

            submissionsResponses = submissionsResponses
                .AsQueryable()
                .OrderBy($"{sortField} {order}")
                .ToList();

            var response = new SubmissionsWithProblemsResponse(
                Submissions: submissionsResponses,
                ProblemIndexes: indexes,
                Properties: typeof(SubmissionsResponse).GetProperties().Select(p => p.Name).ToList());

            return Ok(response);
        }
    }
}