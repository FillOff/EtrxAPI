using System.Net.Http.Json;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;

namespace Etrx.API.Services;

public sealed class CodeforcesProgressClient : ICodeforcesProgressClient
{
    private readonly HttpClient _httpClient;

    private static readonly SemaphoreSlim RequestGate = new SemaphoreSlim(1, 1);
    private static readonly TimeSpan RequestInterval = TimeSpan.FromMilliseconds(2100);
    private static DateTimeOffset _nextRequestAt;
    public const int PageSize = 1000;
    private const string UserStatusEndpoint = "https://codeforces.com/api/user.status";

    public CodeforcesProgressClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Dictionary<(int ContestId, string Index), ProgressFacts>> GetHistoryAsync(
        string handle, CancellationToken cancellationToken)
    {
        var progressByProblem = new Dictionary<(int, string), ProgressFacts>();
        for (var firstSubmission = 1; ; firstSubmission += PageSize)
        {
            await RequestGate.WaitAsync(cancellationToken);
            CodeforcesProgressResponse? response;
            try
            {
                var delay = _nextRequestAt - DateTimeOffset.UtcNow;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken);
                }
                var url = $"{UserStatusEndpoint}?handle={Uri.EscapeDataString(handle)}&from={firstSubmission}&count={PageSize}";
                response = await _httpClient.GetFromJsonAsync<CodeforcesProgressResponse>(url, cancellationToken);
            }
            finally
            {
                _nextRequestAt = DateTimeOffset.UtcNow.Add(RequestInterval);
                RequestGate.Release();
            }
            if (response?.Status != CodeforcesResponseStatuses.Ok || response.Result is null)
            {
                throw new HttpRequestException("Codeforces did not return a complete submission page.");
            }
            foreach (var submission in response.Result)
            {
                var problem = submission.Problem;
                if (problem is null || problem.ContestId <= 0 || string.IsNullOrEmpty(problem.Index))
                {
                    continue;
                }
                var problemKey = (problem.ContestId, problem.Index);
                progressByProblem.TryGetValue(problemKey, out var previousProgress);
                var isSolved = previousProgress.IsSolved || submission.Verdict == Verdicts.Ok;
                var hasFailedAttempt = previousProgress.HasFailedAttempt ||
                    ProblemStatuses.FailedVerdicts.Contains(submission.Verdict ?? string.Empty);
                progressByProblem[problemKey] = new ProgressFacts(isSolved, hasFailedAttempt);
            }
            if (response.Result.Count < PageSize)
            {
                return progressByProblem;
            }
        }
    }
}
