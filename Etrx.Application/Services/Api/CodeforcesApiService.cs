using Etrx.Application.Exceptions.Api;
using Etrx.Application.Interfaces.Api;
using Etrx.Application.Options;
using Etrx.Domain.Models.ParsingModels.Codeforces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Etrx.Application.Services.Api;

public class CodeforcesApiService : ApiService, ICodeforcesApiService
{
    private readonly CodeforcesOptions _codeforcesOptions;

    public CodeforcesApiService(
        HttpClient httpClient, 
        IOptions<CodeforcesOptions> options)
        : base(httpClient)
    {
        _codeforcesOptions = options.Value;
    }

    public async Task<List<CodeforcesUser>> GetCodeforcesUsersAsync(string handlesString)
    {
        var result = await HandleRequestAsync<List<CodeforcesUser>>(
            $"https://codeforces.com/api/user.info?handles={handlesString}&lang=ru&checkHistoricHandles=true");

        return result;
    }

    public async Task<CodeforcesProblemSetResult> GetCodeforcesProblemsAsync(string lang)
    {
        var result = await HandleRequestAsync<CodeforcesProblemSetResult>(
            $"https://codeforces.com/api/problemset.problems?lang={lang}");

        return result;
    }

    public async Task<List<CodeforcesContest>> GetCodeforcesContestsAsync(bool gym, string lang)
    {
        var result = await HandleRequestAsync<List<CodeforcesContest>>(
            $"https://codeforces.com/api/contest.list?gym={gym}&lang={lang}");
        
        return result;
    }

    public async Task<List<CodeforcesSubmission>> GetCodeforcesSubmissionsAsync(string handle)
    {
        var parameters = new Dictionary<string, string> { { "handle", handle } };
        var url = BuildUrl("user.status", parameters);
        var result = await HandleRequestAsync<List<CodeforcesSubmission>>(url);

        return result;
    }

    public async Task<List<CodeforcesSubmission>> GetCodeforcesContestSubmissionsAsync(string handle, int contestId)
    {
        var result = await HandleRequestAsync<List<CodeforcesSubmission>>(
            $"https://codeforces.com/api/contest.status?contestId={contestId}&handle={handle}");

        return result;
    }

    public async Task<List<string>> GetCodeforcesContestUsersAsync(List<string> handles, int contestId, bool isGym)
    {
        var result = await GetCodeforcesRanklistRowsAsync(handles, contestId, isGym);
        var searchSet = new HashSet<string>(handles, StringComparer.OrdinalIgnoreCase);

        return result.Rows
            .SelectMany(row => row.Party.Members)
            .Select(member => member.Handle)
            .Where(searchSet.Contains)
            .Distinct()
            .ToList();
    }

    public async Task<CodeforcesContestStanding> GetCodeforcesRanklistRowsAsync(List<string> handles, int contestId, bool isGym)
    {
        CodeforcesContestStanding result;
        var searchSet = new HashSet<string>(handles, StringComparer.OrdinalIgnoreCase);

        if (isGym)
        {
            var parameters = new Dictionary<string, string>
        {
            { "contestId", contestId.ToString() },
            { "handles", string.Join(";", handles) },
            { "showUnofficial", "true" }
        };

            var url = BuildUrl("contest.standings", parameters);
            result = await HandleRequestAsync<CodeforcesContestStanding>(url);
        }
        else
        {
            var url = $"https://codeforces.com/api/contest.standings?contestId={contestId}";
            result = await HandleRequestAsync<CodeforcesContestStanding>(url);
        }

        result.Rows = result.Rows
            .Where(row => row.Party.Members.Any(m => searchSet.Contains(m.Handle)))
            .ToList();

        return result;
    }


    private string BuildUrl(string methodName, Dictionary<string, string> parameters)
    {
        parameters["apiKey"] = _codeforcesOptions.Key;
        parameters["time"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var sortedParams = parameters
            .OrderBy(p => p.Key)
            .ThenBy(p => p.Value)
            .Select(p => $"{p.Key}={p.Value}")
            .ToList();

        var queryString = string.Join("&", sortedParams);
        var rand = GenerateRandomString(6);
        var toHash = $"{rand}/{methodName}?{queryString}#{_codeforcesOptions.Secret}";
        var hash = ComputeSha512Hash(toHash);
        var apiSig = $"{rand}{hash}";

        var url = $"https://codeforces.com/api/{methodName}?{queryString}&apiSig={apiSig}";
        
        return url;
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string ComputeSha512Hash(string input)
    {
        using var sha512 = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha512.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    protected override async Task<TResult> HandleRequestAsync<TResult>(string url)
    {
        using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

        try
        {
            var apiResponse = await DeserializeAsync<CodeforcesResponse<TResult>>(response);

            if (apiResponse.Status != "OK" || apiResponse.Result is null)
            {
                throw new CodeforcesApiException(
                    apiResponse.Comment,
                    response.StatusCode,
                    url);
            }

            return apiResponse.Result;
        }
        catch (JsonException)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new ApiException("Failed to parse Codeforces response", response.StatusCode, url, body);
        }
    }
}
