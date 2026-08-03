using Etrx.Application.Interfaces.Api;
using Etrx.Application.Options;
using Etrx.Domain.Models.Parsing_models.IoiCodeforces;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services.Api;

public class IoiCodeforcesApiService : ApiService, IIoiCodeforcesApiService
{
    private const string GroupUrl = "https://ioi.contest.codeforces.com/group/32KGsXgiKA";
    private readonly CodeforcesOptions _options;
    private readonly ILogger<IoiCodeforcesApiService> _logger;
    private static readonly SemaphoreSlim AuthenticationLock = new(1, 1);
    private static bool _isAuthenticated;
    private static string? _cachedCsrfToken;

    public IoiCodeforcesApiService(
        HttpClient httpClient,
        IOptions<CodeforcesOptions> options,
        ILogger<IoiCodeforcesApiService> logger)
        : base(httpClient)
    {
        _options = options.Value;
        _logger = logger;
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
    }

    protected override Task<TResult> HandleRequestAsync<TResult>(string url)
    {
        return HandleRequestAsync<TResult>(url, requireAuthentication: true);
    }

    private async Task<TResult> HandleRequestAsync<TResult>(string url, bool requireAuthentication)
    {
        if (requireAuthentication)
        {
            await EnsureAuthenticatedAsync();
        }

        var (response, htmlContent) = await SendRequestAsync(url);

        if (requireAuthentication && IsLoginPage(response, htmlContent))
        {
            response.Dispose();
            _logger.LogWarning("Codeforces authentication expired for {Url}; re-authenticating", url);
            await ReauthenticateAsync(force: true);
            (response, htmlContent) = await SendRequestAsync(url);
        }

        using (response)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                _logger.LogWarning(
                    "Codeforces rejected request {Url} with HTTP 403; authentication required: {RequireAuthentication}",
                    url,
                    requireAuthentication);
                throw new HttpRequestException(
                    $"IOI Codeforces rejected the request with HTTP 403: {url}",
                    null,
                    HttpStatusCode.Forbidden);
            }

            response.EnsureSuccessStatusCode();
        }

        return typeof(TResult) switch
        {
            var t when t == typeof(IoiCodeforcesStandings) => (TResult)(object)ParseStandings(htmlContent),
            var t when t == typeof(List<IoiCodeforcesContest>) => (TResult)(object)ParseContests(htmlContent),
            _ => throw new NotSupportedException()
        };
    }

    private async Task<(HttpResponseMessage Response, string Html)> SendRequestAsync(string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Referrer = new Uri(GroupUrl);
        if (!string.IsNullOrWhiteSpace(_cachedCsrfToken))
        {
            request.Headers.TryAddWithoutValidation("X-Csrf-Token", _cachedCsrfToken);
        }

        var response = await _httpClient.SendAsync(request);
        var html = await response.Content.ReadAsStringAsync();
        _logger.LogInformation(
            "Codeforces request {Url} returned HTTP {StatusCode}; final URI: {FinalUri}",
            url,
            (int)response.StatusCode,
            response.RequestMessage?.RequestUri);
        return (response, html);
    }

    private static bool IsLoginPage(HttpResponseMessage response, string html)
    {
        var path = response.RequestMessage?.RequestUri?.AbsolutePath;
        return response.StatusCode == HttpStatusCode.Forbidden
            || string.Equals(path, "/enter", StringComparison.OrdinalIgnoreCase)
            || html.Contains("id=\"enterForm\"", StringComparison.OrdinalIgnoreCase)
            || html.Contains("name=\"handleOrEmail\"", StringComparison.OrdinalIgnoreCase);
    }

    private async Task ReauthenticateAsync(bool force = false)
    {
        await AuthenticationLock.WaitAsync();
        try
        {
            if (!force && _isAuthenticated)
            {
                return;
            }

            _cachedCsrfToken = null;
            await LoginAsync();
            _isAuthenticated = true;
        }
        finally
        {
            AuthenticationLock.Release();
        }
    }


    private List<IoiCodeforcesContest> ParseContests(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var contests = new List<IoiCodeforcesContest>();
        var rows = doc.DocumentNode.SelectNodes("//tr[@data-contestid or @data-contestId]");

        if (rows != null)
        {
            foreach (var row in rows)
            {
                var nameNode = row.SelectSingleNode("./td[1]/text()[normalize-space()][1]");
                var contestId = row.GetAttributeValue("data-contestid", "");
                if (string.IsNullOrWhiteSpace(contestId))
                {
                    contestId = row.GetAttributeValue("data-contestId", "");
                }
                var durationNode = row.SelectSingleNode("./td[3]");

                if (nameNode != null)
                {
                    string cleanName = CleanText(nameNode.InnerText);

                    contests.Add(new IoiCodeforcesContest
                    {
                        Id = contestId,
                        Name = cleanName,
                        Duration = CleanText(durationNode?.InnerText),
                        StartTime = CleanText(row.SelectSingleNode("./td[2]")?.InnerText)
                    });
                }
            }
        }

        return contests;
    }

    private IoiCodeforcesStandings ParseStandings(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var result = new IoiCodeforcesStandings();

        var nameNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'contest-name')]/a");
        result.ContestName = CleanText(nameNode?.InnerText) is { Length: > 0 } name
            ? name
            : "Unknown";
        var problemIndexes = doc.DocumentNode.SelectNodes(
                "//table[contains(@class, 'standings')]//tr[1]/th/a[contains(@href, '/problem/')]")?
            .Select(node => new
            {
                Index = CleanText(node.InnerText),
                Url = node.GetAttributeValue("href", string.Empty)
            })
            .ToList() ?? [];

        var rows = doc.DocumentNode.SelectNodes("//tr[@participantid]");
        if (rows != null)
        {
            foreach (var row in rows)
            {
                var participant = new IoiCodeforcesParticipantRow();

                participant.Rank = CleanText(row.SelectSingleNode("./td[1]")?.InnerText);

                var handleNode = row.SelectSingleNode(".//td[contains(@class, 'contestant-cell')]//a");
                participant.Handle = CleanText(handleNode?.InnerText);

                participant.TotalScore = ParseDouble(row.SelectSingleNode("./td[3]")?.InnerText);

                var problemCells = row.SelectNodes("./td[@problemid]");
                if (problemCells != null)
                {
                    for (var i = 0; i < problemCells.Count; i++)
                    {
                        var problem = i < problemIndexes.Count
                            ? problemIndexes[i]
                            : new { Index = string.Empty, Url = string.Empty };
                        participant.ProblemScores.Add(ParseProblemResult(problemCells[i], problem.Index, problem.Url));
                    }
                }

                result.Rows.Add(participant);
            }
        }

        return result;
    }

    public async Task<List<IoiCodeforcesContest>> GetContestsAsync()
    {
        return await HandleRequestAsync<List<IoiCodeforcesContest>>(
            $"{GroupUrl}/contests",
            requireAuthentication: false);
    }

    public async Task<IoiCodeforcesStandings> GetRanklistRowsByContestIdAsync(int contestId)
    {
        await EnsureAuthenticatedAsync();

        try
        {
            return await GetStandingsPageAsync(contestId);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("HTTP 403", StringComparison.Ordinal))
        {
            // Codeforces can invalidate the session after a while. Refresh it once and retry.
            await AuthenticationLock.WaitAsync();
            try
            {
                _isAuthenticated = false;
                _cachedCsrfToken = null;
            }
            finally
            {
                AuthenticationLock.Release();
            }

            await EnsureAuthenticatedAsync();
            return await GetStandingsPageAsync(contestId);
        }
    }

    private Task<IoiCodeforcesStandings> GetStandingsPageAsync(int contestId)
    {
        return HandleRequestAsync<IoiCodeforcesStandings>(
            $"{GroupUrl}/contest/{contestId}/standings/friends/true");
    }

    private async Task EnsureAuthenticatedAsync()
    {
        if (_isAuthenticated)
        {
            return;
        }

        await AuthenticationLock.WaitAsync();
        try
        {
            if (!_isAuthenticated)
            {
                await LoginAsync();
                _isAuthenticated = true;
            }
        }
        finally
        {
            AuthenticationLock.Release();
        }
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(_options.Login)
            || string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException(
                "Codeforces credentials are not configured. Set Codeforces:Login and Codeforces:Password.");
        }

        using var loginPageRequest = new HttpRequestMessage(
            HttpMethod.Get,
            "https://ioi.contest.codeforces.com/enter");
        loginPageRequest.Headers.Referrer = new Uri(GroupUrl);
        var loginPage = await _httpClient.SendAsync(loginPageRequest);
        loginPage.EnsureSuccessStatusCode();
        var loginHtml = await loginPage.Content.ReadAsStringAsync();

        var finalLoginUri = loginPage.RequestMessage?.RequestUri;
        var isLoginForm = loginHtml.Contains("name=\"handleOrEmail\"", StringComparison.OrdinalIgnoreCase)
            || loginHtml.Contains("id=\"enterForm\"", StringComparison.OrdinalIgnoreCase);
        var isAuthenticatedPage = loginHtml.Contains("/logout", StringComparison.OrdinalIgnoreCase)
            || loginHtml.Contains(_options.Login, StringComparison.OrdinalIgnoreCase);

        if (!isLoginForm && (isAuthenticatedPage || finalLoginUri?.AbsolutePath != "/enter"))
        {
            _cachedCsrfToken = ExtractCsrfToken(loginHtml);
            _isAuthenticated = true;
            return;
        }

        var document = new HtmlDocument();
        document.LoadHtml(loginHtml);
        var csrfToken = document.DocumentNode
            .SelectSingleNode("//input[@name='csrf_token']")?
            .GetAttributeValue("value", string.Empty);

        if (string.IsNullOrWhiteSpace(csrfToken))
        {
            csrfToken = document.DocumentNode
                .SelectSingleNode("//meta[@name='X-Csrf-Token']")?
                .GetAttributeValue("content", string.Empty);
        }

        if (string.IsNullOrWhiteSpace(csrfToken))
        {
            csrfToken = document.DocumentNode
                .SelectSingleNode("//*[contains(@class, 'csrf-token')]")?
                .GetAttributeValue("data-csrf", string.Empty);
        }

        if (string.IsNullOrWhiteSpace(csrfToken))
        {
            throw new InvalidOperationException("Codeforces login page did not contain a CSRF token.");
        }

        var ftaa = Guid.NewGuid().ToString("N")[..18];
        var bfaa = Guid.NewGuid().ToString("N");
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["csrf_token"] = csrfToken,
            ["action"] = "enter",
            ["ftaa"] = ftaa,
            ["bfaa"] = bfaa,
            ["handleOrEmail"] = _options.Login,
            ["password"] = _options.Password,
            ["remember"] = "on"
        });

        using var loginRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "https://ioi.contest.codeforces.com/enter")
        {
            Content = content
        };
        loginRequest.Headers.Referrer = new Uri("https://ioi.contest.codeforces.com/enter");
        loginRequest.Headers.TryAddWithoutValidation("Origin", "https://ioi.contest.codeforces.com");
        loginRequest.Headers.TryAddWithoutValidation("X-Csrf-Token", csrfToken);

        var response = await _httpClient.SendAsync(loginRequest);
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            _logger.LogWarning(
                "Codeforces rejected login for {LoginUrl} with HTTP {StatusCode}; final URI: {FinalUri}",
                loginRequest.RequestUri,
                (int)response.StatusCode,
                response.RequestMessage?.RequestUri);
            throw new UnauthorizedAccessException(
                "IOI Codeforces rejected the login request with HTTP 403. Check the configured credentials and container network access.");
        }

        var responseHtml = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Codeforces login failed with HTTP {StatusCode}; final URI: {FinalUri}",
                (int)response.StatusCode,
                response.RequestMessage?.RequestUri);
            throw new HttpRequestException(
                $"Codeforces login failed with HTTP {(int)response.StatusCode}",
                null,
                response.StatusCode);
        }

        if (IsLoginPage(response, responseHtml)
            || Regex.IsMatch(responseHtml, "Invalid handle or password|Invalid password", RegexOptions.IgnoreCase))
        {
            throw new UnauthorizedAccessException("Codeforces rejected the configured credentials.");
        }

        _cachedCsrfToken = ExtractCsrfToken(responseHtml) ?? csrfToken;
    }

    private static string? ExtractCsrfToken(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        return document.DocumentNode
                   .SelectSingleNode("//input[@name='csrf_token']")?
                   .GetAttributeValue("value", string.Empty)
               ?? document.DocumentNode
                   .SelectSingleNode("//meta[@name='X-Csrf-Token']")?
                   .GetAttributeValue("content", string.Empty)
               ?? document.DocumentNode
                   .SelectSingleNode("//*[contains(@class, 'csrf-token')]")?
                   .GetAttributeValue("data-csrf", string.Empty);
    }

    private static string CleanText(string? value)
    {
        return WebUtility.HtmlDecode(value ?? string.Empty)
            .Replace('\u00a0', ' ')
            .Trim();
    }

    private static IoiCodeforcesProblemResult ParseProblemResult(HtmlNode cell, string problemIndex, string problemUrl)
    {
        var text = CleanText(cell.InnerText);
        return new IoiCodeforcesProblemResult
        {
            Index = problemIndex,
            Points = ParseDouble(text),
            Url = problemUrl
        };
    }


    private static double ParseDouble(string? value)
    {
        var match = Regex.Match(CleanText(value), @"-?\d+(?:[.,]\d+)?");
        return match.Success && double.TryParse(match.Value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
            ? number
            : 0;
    }
}
