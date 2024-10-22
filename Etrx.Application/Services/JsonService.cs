using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using System.Text.Json;

namespace Etrx.Application.Services
{
    public class JsonService : IJsonService
    {
        private readonly IUsersRepository _usersRepository;

        public JsonService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public Submission JsonToSubmission(JsonElement jsonSubmission)
        {
            ulong id = jsonSubmission.GetProperty("id").GetUInt64();
            int? contestId = jsonSubmission.TryGetProperty("contestId", out var contestIdProp) ? contestIdProp.ValueKind != JsonValueKind.Null ? contestIdProp.GetInt32() : null : null;
            string index = jsonSubmission.GetProperty("problem").GetProperty("index").ToString();
            DateTime creationTimeSeconds = DateTimeOffset.FromUnixTimeSeconds(jsonSubmission.GetProperty("creationTimeSeconds").GetInt64()).UtcDateTime;
            DateTime relativeTimeSeconds = DateTimeOffset.FromUnixTimeSeconds(jsonSubmission.GetProperty("creationTimeSeconds").GetInt64()).UtcDateTime;
            string programmingLanguage = jsonSubmission.GetProperty("programmingLanguage").ToString()!;
            string? verdict = jsonSubmission.TryGetProperty("verdict", out var verdictProp) ? verdictProp.ValueKind != JsonValueKind.Null ? verdictProp.ToString() : null : null;
            string testset = jsonSubmission.GetProperty("testset").ToString()!;
            string participantType = jsonSubmission.GetProperty("author").GetProperty("participantType").ToString()!;
            int passedTestCount = jsonSubmission.GetProperty("passedTestCount").GetInt32();
            int timeConsumedMillis = jsonSubmission.GetProperty("timeConsumedMillis").GetInt32();
            long memoryConsumedBytes = jsonSubmission.GetProperty("memoryConsumedBytes").GetInt64();

            var handles = jsonSubmission.GetProperty("author").GetProperty("members").EnumerateArray();

            foreach (var jsonHandle in handles)
            {
                var handle = jsonHandle.GetProperty("handle").ToString();
                var user = _usersRepository.GetByHandle(handle);
                if (user != null)
                {
                    var submission = new Submission(id, contestId, index, creationTimeSeconds, relativeTimeSeconds, programmingLanguage,
                                                    handle, user.FirstName!, user.LastName!, participantType, verdict, testset, passedTestCount, 
                                                    timeConsumedMillis, memoryConsumedBytes);

                    return submission;
                }
            }
            return null!;
        }
    }
}