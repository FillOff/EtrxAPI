using Etrx.Domain.Models;
using System.Text.Json;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IJsonService
    {
        Problem JsonToProblem(JsonElement jsonProblem, JsonElement.ArrayEnumerator statistics);
        Contest JsonToContest(JsonElement jsonContest, bool gym);
        User JsonToUser(string handle, string? firstName, string? lastName, int? grade, JsonElement jsonUser);
        Submission JsonToSubmission(JsonElement jsonSubmission);
    }
}