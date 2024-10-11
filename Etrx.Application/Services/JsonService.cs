using Etrx.Core.Models;
using Etrx.Domain.Interfaces.Repositories;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using System.Net.Http.Json;
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

        public Problem JsonToProblem(JsonElement jsonProblem, JsonElement.ArrayEnumerator statistics)
        {
            int contestId = jsonProblem.GetProperty("contestId").GetInt32();
            string index = jsonProblem.GetProperty("index").GetString()!;
            string name = jsonProblem.GetProperty("name").GetString()!;
            string type = jsonProblem.GetProperty("type").GetString()!;
            double? points = jsonProblem.TryGetProperty("points", out var pointsProperty) ? pointsProperty.ValueKind != JsonValueKind.Null ? pointsProperty.GetDouble() : null : null;
            int? rating = jsonProblem.TryGetProperty("rating", out var ratingProperty) ? ratingProperty.ValueKind != JsonValueKind.Null ? ratingProperty.GetInt32() : null : null;
            string[] tags = jsonProblem.GetProperty("tags").EnumerateArray().Select(tag => tag.GetString()).ToArray()!;

            int? solvedCount = statistics.First(stat =>
                    stat.GetProperty("contestId").GetInt32() == contestId &&
                    stat.GetProperty("index").GetString() == index)
                        .GetProperty("solvedCount").GetInt32();

            var problem = new Problem(0, contestId, index, name, type, points, rating, solvedCount, tags);

            return problem;
        }

        public Contest JsonToContest(JsonElement jsonContest, bool gym)
        {
            int contestId = jsonContest.GetProperty("id").GetInt32();
            string name = jsonContest.GetProperty("name").GetString()!;
            string type = jsonContest.GetProperty("type").GetString()!;
            string phase = jsonContest.GetProperty("phase").GetString()!;
            bool frozen = jsonContest.GetProperty("frozen").GetBoolean();
            int durationSeconds = jsonContest.GetProperty("durationSeconds").GetInt32();
            DateTime? startTimeSeconds = jsonContest.TryGetProperty("startTimeSeconds", out var startTime) ? startTime.ValueKind != JsonValueKind.Null ? DateTimeOffset.FromUnixTimeSeconds(startTime.GetInt64()).UtcDateTime : null : null;
            int? relativeTimeSeconds = jsonContest.TryGetProperty("relativeTimeSeconds", out var relativeTime) ? relativeTime.GetInt32() : (int?)null;
            string? preparedBy = jsonContest.TryGetProperty("preparedBy", out var prepared) ? prepared.ValueKind != JsonValueKind.Null ? prepared.GetString() : null : null;
            string? websiteUrl = jsonContest.TryGetProperty("websiteUrl", out var website) ? website.ValueKind != JsonValueKind.Null ? website.GetString() : null : null;
            string? description = jsonContest.TryGetProperty("description", out var desc) ? desc.ValueKind != JsonValueKind.Null ? desc.GetString() : null : null;
            int? difficulty = jsonContest.TryGetProperty("difficulty", out var diff) ? diff.ValueKind != JsonValueKind.Null ? diff.GetInt32() : null : null;
            string? kind = jsonContest.TryGetProperty("kind", out var kindProperty) ? kindProperty.ValueKind != JsonValueKind.Null ? kindProperty.GetString() : null : null;
            string? icpcRegion = jsonContest.TryGetProperty("icpcRegion", out var region) ? region.ValueKind != JsonValueKind.Null ? region.GetString() : null : null;
            string? country = jsonContest.TryGetProperty("country", out var countryProperty) ? countryProperty.ValueKind != JsonValueKind.Null ? countryProperty.GetString() : null : null;
            string? city = jsonContest.TryGetProperty("city", out var cityProperty) ? cityProperty.ValueKind != JsonValueKind.Null ? cityProperty.GetString() : null : null;
            string? season = jsonContest.TryGetProperty("season", out var pointsProperty) ? pointsProperty.ValueKind != JsonValueKind.Null ? pointsProperty.GetString() : null : null;

            var contest = new Contest(contestId, name, type, phase, frozen, durationSeconds, startTimeSeconds, relativeTimeSeconds, preparedBy, websiteUrl, description, difficulty, kind, icpcRegion, country, city, season, gym);

            return contest;
        }

        public User JsonToUser(string handle, string? firstName, string? lastName, int? grade, JsonElement jsonUser)
        {
            firstName ??= jsonUser.TryGetProperty("firstName", out var firstNameProperty) ? firstNameProperty.ValueKind != JsonValueKind.Null ? firstNameProperty.ToString() : null : null;
            lastName ??= jsonUser.TryGetProperty("lastName", out var lastNameProperty) ? lastNameProperty.ValueKind != JsonValueKind.Null ? lastNameProperty.ToString() : null : null;
            grade ??= jsonUser.TryGetProperty("grade", out var gradeProperty) ? gradeProperty.ValueKind != JsonValueKind.Null ? gradeProperty.GetInt32() : null : null;

            string? email = jsonUser.TryGetProperty("email", out var email1) ? email1.ValueKind != JsonValueKind.Null ? email1.ToString() : null : null;
            string? vkId = jsonUser.TryGetProperty("vkId", out var vkId1) ? vkId1.ValueKind != JsonValueKind.Null ? vkId1.ToString() : null : null;
            string? openId = jsonUser.TryGetProperty("openId", out var openId1) ? openId1.ValueKind != JsonValueKind.Null ? openId1.ToString() : null : null;
            string? country = jsonUser.TryGetProperty("country", out var country1) ? country1.ValueKind != JsonValueKind.Null ? country1.ToString() : null : null;
            string? city = jsonUser.TryGetProperty("city", out var city1) ? city1.ValueKind != JsonValueKind.Null ? city1.ToString() : null : null;
            string? organization = jsonUser.TryGetProperty("organization", out var organization1) ? organization1.ValueKind != JsonValueKind.Null ? organization1.ToString() : null : null;
            int? contribution = jsonUser.TryGetProperty("contribution", out var contribution1) ? contribution1.ValueKind != JsonValueKind.Null ? contribution1.GetInt32() : null : null;
            string? rank = jsonUser.TryGetProperty("rank", out var rank1) ? rank1.ValueKind != JsonValueKind.Null ? rank1.ToString() : null : null;
            int? rating = jsonUser.TryGetProperty("rating", out var rating1) ? rating1.ValueKind != JsonValueKind.Null ? rating1.GetInt32() : null : null;
            string? maxRank = jsonUser.TryGetProperty("maxRank", out var maxRank1) ? maxRank1.ValueKind != JsonValueKind.Null ? maxRank1.ToString() : null : null;
            int? maxRating = jsonUser.TryGetProperty("maxRating", out var maxRating1) ? maxRating1.ValueKind != JsonValueKind.Null ? maxRating1.GetInt32() : null : null;
            DateTime? lastOnlineTimeSeconds = jsonUser.TryGetProperty("lastOnlineTimeSeconds", out var lastOnlineTimeSeconds1) ? lastOnlineTimeSeconds1.ValueKind != JsonValueKind.Null ? DateTimeOffset.FromUnixTimeSeconds(lastOnlineTimeSeconds1.GetInt64()).UtcDateTime : null : null;
            DateTime? registrationTimeSeconds = jsonUser.TryGetProperty("registrationTimeSeconds", out var registrationTimeSeconds1) ? registrationTimeSeconds1.ValueKind != JsonValueKind.Null ? DateTimeOffset.FromUnixTimeSeconds(registrationTimeSeconds1.GetInt64()).UtcDateTime : null : null;
            int? friendOfCount = jsonUser.TryGetProperty("friendOfCount", out var friendOfCount1) ? friendOfCount1.ValueKind != JsonValueKind.Null ? friendOfCount1.GetInt32() : null : null;
            string? avatar = jsonUser.TryGetProperty("avatar", out var avatar1) ? avatar1.ValueKind != JsonValueKind.Null ? avatar1.ToString() : null : null;
            string? titlePhoto = jsonUser.TryGetProperty("titlePhoto", out var titlePhoto1) ? titlePhoto1.ValueKind != JsonValueKind.Null ? titlePhoto1.ToString() : null : null;

            var user = new User(0, handle, email, vkId, openId, firstName, lastName, country, city, organization, contribution, rank, rating,
                                maxRank, maxRating, lastOnlineTimeSeconds, registrationTimeSeconds, friendOfCount, avatar, titlePhoto, grade);

            return user;
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
            int passedTestCount = jsonSubmission.GetProperty("passedTestCount").GetInt32();
            int timeConsumedMillis = jsonSubmission.GetProperty("timeConsumedMillis").GetInt32();
            long memoryConsumedBytes = jsonSubmission.GetProperty("memoryConsumedBytes").GetInt64();

            var handles = jsonSubmission.GetProperty("author").GetProperty("members").EnumerateArray();
            string name;

            foreach (var jsonHandle in handles)
            {
                var handle = jsonHandle.GetProperty("handle").ToString();
                var user = _usersRepository.GetByHandle(handle);
                if (user != null)
                {
                    name = $"{user.FirstName} {user.LastName}";
                    var submission = new Submission(id, contestId, index, creationTimeSeconds, relativeTimeSeconds, programmingLanguage,
                                                    handle, name, verdict, testset, passedTestCount, timeConsumedMillis, memoryConsumedBytes);

                    return submission;
                }
            }
            return null!;
        }
    }
}
