using Etrx.Application.Constants;
using Etrx.Application.Interfaces;
using Etrx.Application.Interfaces.Api;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services;

public class UpdateDataService : IUpdateDataService
{
    private readonly ILogger<UpdateDataService> _logger;
    private readonly ICodeforcesApiService _codeforcesApiService;
    private readonly ICodeforcesService _codeforcesService;
    private readonly IDlApiService _dlApiService;
    private readonly IUsersService _usersService;

    public UpdateDataService(
        ILogger<UpdateDataService> logger,
        ICodeforcesApiService codeforcesApiService,
        ICodeforcesService codeforcesService,
        IDlApiService dlApiService,
        IUsersService usersService)
    {
        _logger = logger;
        _codeforcesApiService = codeforcesApiService;
        _codeforcesService = codeforcesService;
        _dlApiService = dlApiService;
        _usersService = usersService;
    }

    public async Task UpdateProblemsAsync()
    {
        var (Problems, ProblemStatistics) = await _codeforcesApiService.GetCodeforcesProblemsAsync(Languages.Ru);
        await _codeforcesService.PostProblemsFromCodeforcesAsync(Problems!, ProblemStatistics!, Languages.Ru);

        (Problems, ProblemStatistics) = await _codeforcesApiService.GetCodeforcesProblemsAsync(Languages.En);
        await _codeforcesService.PostProblemsFromCodeforcesAsync(Problems!, ProblemStatistics!, Languages.En);

        _logger.LogInformation($"Problems updated successfully.");
    }

    public async Task UpdateContestsAsync()
    {
        var contests = await _codeforcesApiService.GetCodeforcesContestsAsync(false, Languages.Ru);
        await _codeforcesService.PostContestsFromCodeforcesAsync(contests!, false, Languages.Ru);

        contests = await _codeforcesApiService.GetCodeforcesContestsAsync(true, Languages.Ru);
        await _codeforcesService.PostContestsFromCodeforcesAsync(contests!, true, Languages.Ru);

        contests = await _codeforcesApiService.GetCodeforcesContestsAsync(false, Languages.En);
        await _codeforcesService.PostContestsFromCodeforcesAsync(contests!, false, Languages.En);

        contests = await _codeforcesApiService.GetCodeforcesContestsAsync(true, Languages.En);
        await _codeforcesService.PostContestsFromCodeforcesAsync(contests!, true, Languages.En);

        _logger.LogInformation($"Contests updated successfully.");
    }

    public async Task UpdateUsersAsync()
    {
        var dlUsers = await _dlApiService.GetDlUsersAsync();
        foreach ( var dlUser in dlUsers )
        {
            var handle = dlUser.Handle;
            var user = await _codeforcesApiService.GetCodeforcesUsersAsync(handle);

            await _codeforcesService.PostUserFromDlCodeforcesAsync(dlUser, user[0]);
            await Task.Delay(2000);
        }

        _logger.LogInformation($"Users updated successfully.");
    }

    public async Task UpdateSubmissionsAsync()
    {
        var handles = await _usersService.GetHandlesAsync();
        foreach (var handle in handles)
        {
            var submissions = await _codeforcesApiService.GetCodeforcesSubmissionsAsync(handle);
            await _codeforcesService.PostSubmissionsFromCodeforcesAsync(submissions, handle);
            await Task.Delay(2000);
        }

        _logger.LogInformation($"Submissions updated successfully.");
    }

    public async Task UpdateSubmissionsByContestIdAsync(int contestId)
    {
        var handles = await _codeforcesApiService.GetCodeforcesContestUsersAsync(await _usersService.GetHandlesAsync(), contestId);
        foreach(var handle in handles)
        {
            var submissions = await _codeforcesApiService.GetCodeforcesContestSubmissionsAsync(handle, contestId);
            await _codeforcesService.PostSubmissionsFromCodeforcesAsync(submissions, handle);
            await Task.Delay(2000);
        }

        _logger.LogInformation("Submissions updated successfully (contestId: {ContestId}).", contestId);
    }

    public async Task UpdateRanklistRowsByContestIdAsync(int contestId)
    {
        var response = await _codeforcesApiService.GetCodeforcesRanklistRowsAsync(await _usersService.GetHandlesAsync(), contestId);
        await _codeforcesService.PostRanklistRowsFromCodeforcesAsync(response);

        _logger.LogInformation("RanklistRows updated successfully (contestId: {ContestId}).", contestId);
    }
}