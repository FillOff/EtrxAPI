using Etrx.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services;

public class UpdateDataService : IUpdateDataService
{
    private readonly ILogger<UpdateDataService> _logger;
    private readonly ILastUpdateTimeService _lastTimeUpdateService;
    private readonly ICodeforcesApiService _codeforcesApiService;
    private readonly ICodeforcesService _codeforcesService;
    private readonly IDlApiService _dlApiService;
    private readonly IUsersService _usersService;

    public UpdateDataService(
        ILogger<UpdateDataService> logger,
        ILastUpdateTimeService lastTimeUpdateService,
        ICodeforcesApiService codeforcesApiService,
        ICodeforcesService codeforcesService,
        IDlApiService dlApiService,
        IUsersService usersService)
    {
        _logger = logger;
        _lastTimeUpdateService = lastTimeUpdateService;
        _codeforcesApiService = codeforcesApiService;
        _codeforcesService = codeforcesService;
        _dlApiService = dlApiService;
        _usersService = usersService;
    }

    public async Task UpdateProblems()
    {
        var (Problems, ProblemStatistics) = await _codeforcesApiService.GetCodeforcesProblemsAsync("ru");
        await _codeforcesService.PostProblemsFromCodeforces(Problems!, ProblemStatistics!, "ru");

        (Problems, ProblemStatistics) = await _codeforcesApiService.GetCodeforcesProblemsAsync("en");
        await _codeforcesService.PostProblemsFromCodeforces(Problems!, ProblemStatistics!, "en");

        _logger.LogInformation($"Problems updated successfully.");
        _lastTimeUpdateService.UpdateLastUpdateTime("problems", DateTime.Now.AddHours(3));
    }

    public async Task UpdateContests()
    {
        var contests = await _codeforcesApiService.GetCodeforcesContestsAsync(false, "ru");
        await _codeforcesService.PostContestsFromCodeforces(contests!, false, "ru");

        contests = await _codeforcesApiService.GetCodeforcesContestsAsync(true, "ru");
        await _codeforcesService.PostContestsFromCodeforces(contests!, true, "ru");

        contests = await _codeforcesApiService.GetCodeforcesContestsAsync(false, "en");
        await _codeforcesService.PostContestsFromCodeforces(contests!, false, "en");

        contests = await _codeforcesApiService.GetCodeforcesContestsAsync(true, "en");
        await _codeforcesService.PostContestsFromCodeforces(contests!, true, "en");

        _logger.LogInformation($"Contests updated successfully.");
        _lastTimeUpdateService.UpdateLastUpdateTime("contests", DateTime.Now.AddHours(3));
    }

    public async Task UpdateUsers()
    {
        var dlUsers = await _dlApiService.GetDlUsersAsync();
        foreach ( var dlUser in dlUsers )
        {
            var handle = dlUser.Handle;
            var user = await _codeforcesApiService.GetCodeforcesUsersAsync(handle);

            await _codeforcesService.PostUserFromDlCodeforces(dlUser, user[0]);
            await Task.Delay(2000);
        }

        _logger.LogInformation($"Dl users updated successfully.");
        _lastTimeUpdateService.UpdateLastUpdateTime("users", DateTime.Now.AddHours(3));
    }

    public async Task UpdateSubmissions()
    {
        var handles = await _usersService.GetHandlesAsync();
        foreach (var handle in handles)
        {
            var submissions = await _codeforcesApiService.GetCodeforcesSubmissionsAsync(handle);
            await _codeforcesService.PostSubmissionsFromCodeforces(submissions, handle);
            await Task.Delay(2000);
        }

        _logger.LogInformation($"Submissions updated successfully.");
    }

    public async Task UpdateSubmissionsByContestId(int contestId)
    {
        var handles = await _codeforcesApiService.GetCodeforcesContestUsersAsync(await _usersService.GetHandlesAsync(), contestId);
        foreach(var handle in handles)
        {
            var submissions = await _codeforcesApiService.GetCodeforcesContestSubmissionsAsync(handle, contestId);
            await _codeforcesService.PostSubmissionsFromCodeforces(submissions, handle);
            await Task.Delay(2000);
        }

        _logger.LogInformation($"Submissions updated successfully.");
    }

    public async Task UpdateRanklistRowsByContestId(int contestId)
    {
        var response = await _codeforcesApiService.GetCodeforcesRanklistRowsAsync(await _usersService.GetHandlesAsync(), contestId);
        await _codeforcesService.PostRanklistRowsFromCodeforces(response);

        _logger.LogInformation($"RanklistRows updated successfully.");
    }
}
