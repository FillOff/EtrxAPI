using Etrx.Application.Constants;
using Etrx.Application.Interfaces;
using Etrx.Application.Interfaces.Api;
using Etrx.Application.Repositories.UnitOfWork;

namespace Etrx.Application.Services;

public class UpdateDataService : IUpdateDataService
{
    private readonly ICodeforcesApiService _codeforcesApiService;
    private readonly ICodeforcesService _codeforcesService;
    private readonly IIoiCodeforcesApiService _ioiCodeforcesApiService;
    private readonly IIoiCodeforcesService _ioiCodeforcesService;
    private readonly IDlApiService _dlApiService;
    private readonly IUsersService _usersService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDataService(
        ICodeforcesApiService codeforcesApiService,
        ICodeforcesService codeforcesService,
        IIoiCodeforcesApiService ioiCodeforcesApiService,
        IIoiCodeforcesService ioiCodeforcesService,
        IDlApiService dlApiService,
        IUsersService usersService,
        IUnitOfWork unitOfWork)
    {
        _codeforcesApiService = codeforcesApiService;
        _codeforcesService = codeforcesService;
        _ioiCodeforcesApiService = ioiCodeforcesApiService;
        _ioiCodeforcesService = ioiCodeforcesService;
        _dlApiService = dlApiService;
        _usersService = usersService;
        _unitOfWork = unitOfWork;
    }

    public async Task UpdateProblemsAsync()
    {
        var result = await _codeforcesApiService.GetCodeforcesProblemsAsync(Languages.Ru);
        await _codeforcesService.PostProblemsFromCodeforcesAsync(result.Problems!, result.ProblemStatistics!, Languages.Ru);

        result = await _codeforcesApiService.GetCodeforcesProblemsAsync(Languages.En);
        await _codeforcesService.PostProblemsFromCodeforcesAsync(result.Problems!, result.ProblemStatistics!, Languages.En);
    }

    public async Task UpdateContestsAsync()
    {
        var codeforcesContests = await _codeforcesApiService.GetCodeforcesContestsAsync(false, Languages.Ru);
        await _codeforcesService.PostContestsFromCodeforcesAsync(codeforcesContests, false, Languages.Ru);

        codeforcesContests = await _codeforcesApiService.GetCodeforcesContestsAsync(true, Languages.Ru);
        await _codeforcesService.PostContestsFromCodeforcesAsync(codeforcesContests, true, Languages.Ru);

        codeforcesContests = await _codeforcesApiService.GetCodeforcesContestsAsync(false, Languages.En);
        await _codeforcesService.PostContestsFromCodeforcesAsync(codeforcesContests, false, Languages.En);

        codeforcesContests = await _codeforcesApiService.GetCodeforcesContestsAsync(true, Languages.En);
        await _codeforcesService.PostContestsFromCodeforcesAsync(codeforcesContests, true, Languages.En);
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
    }

    public async Task UpdateSubmissionsByContestIdAsync(int contestId)
    {
        var contest = await _unitOfWork.Contests.GetByContestIdAsync(contestId)
            ?? throw new Exception("Contest not found");

        var handles = await _codeforcesApiService.GetCodeforcesContestUsersAsync(await _usersService.GetHandlesAsync(), contestId, contest.Gym);
        foreach(var handle in handles)
        {
            var submissions = await _codeforcesApiService.GetCodeforcesContestSubmissionsAsync(handle, contestId);
            await _codeforcesService.PostSubmissionsFromCodeforcesAsync(submissions, handle);
            await Task.Delay(2000);
        }
    }

    public async Task UpdateRanklistRowsByContestIdAsync(int contestId)
    {
        var contest = await _unitOfWork.Contests.GetByContestIdAsync(contestId)
            ?? throw new Exception("Contest not found");

        var response = await _codeforcesApiService.GetCodeforcesRanklistRowsAsync(await _usersService.GetHandlesAsync(), contestId, contest.Gym);
        if (contest.Gym)
        {
            await _codeforcesService.PostProblemsFromCodeforcesAsync(response.Problems, [], Languages.Ru);
        }
        await _codeforcesService.PostRanklistRowsFromCodeforcesAsync(response);
    }

    public async Task UpdateIoiContestsAsync()
    {
        var contests = await _ioiCodeforcesApiService.GetContestsAsync();
        await _ioiCodeforcesService.PostContestsAsync(contests, Languages.Ru);
        await _ioiCodeforcesService.PostContestsAsync(contests, Languages.En);
    }

    public async Task UpdateIoiRanklistRowsByContestIdAsync(int contestId)
    {
        _ = await _unitOfWork.Contests.GetByContestIdAsync(contestId)
            ?? throw new Exception("Contest not found");

        var ranklistRows = await _ioiCodeforcesApiService.GetRanklistRowsByContestIdAsync(contestId);
        await _ioiCodeforcesService.PostRanklistRowsAsync(contestId, ranklistRows);
    }
}
