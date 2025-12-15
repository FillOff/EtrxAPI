using System.Data.Entity.Infrastructure;
using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Domain.Models;
using Etrx.Domain.Models.ParsingModels.Codeforces;
using Etrx.Domain.Models.ParsingModels.Dl;

namespace Etrx.Application.Services;

public class CodeforcesService : ICodeforcesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CodeforcesService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task PostUserFromDlCodeforces(DlUser dlUser, CodeforcesUser cfUser)
    {
        var existedUser = await _unitOfWork.Users.GetByHandleAsync(cfUser.Handle);

        User userEntity;

        if (existedUser is not null)
        {
            userEntity = existedUser;
            _mapper.Map(cfUser, userEntity);
            _mapper.Map(dlUser, userEntity);
        }
        else
        {
            userEntity = _mapper.Map<User>(cfUser);
            _mapper.Map(dlUser, userEntity);
        }

        await _unitOfWork.Users.InsertOrUpdateAsync([userEntity]);
    }

    public async Task PostProblemsFromCodeforces(List<CodeforcesProblem> problems, List<CodeforcesProblemStatistics> problemStatistics, string languageCode)
    {
        var allTagNames = problems.Where(p => p.Tags != null).SelectMany(p => p.Tags).Distinct().ToList();
        var loadedTags = await _unitOfWork.Tags.GetAllWithTrackingAsync();
        var tagsDict = loadedTags.ToDictionary(t => t.Name);
        var newTagNames = allTagNames.Except(tagsDict.Keys).ToList();

        if (newTagNames.Any())
        {
            foreach (var name in newTagNames)
            {
                var tag = new Tag { Id = Guid.NewGuid(), Name = name, Complexity = 0 };
                await _unitOfWork.Tags.AddAsync(tag);
                tagsDict[name] = tag;
            }
            await _unitOfWork.SaveAsync();
        }

        var contestsDict = (await _unitOfWork.Contests.GetAllAsync()).ToDictionary(c => c.ContestId, c => c.Id);
        var statsDict = problemStatistics.ToDictionary(s => (s.ContestId, s.Index));

        var existingProblems = await _unitOfWork.Problems.GetAllWithTrackingAsync();
        var problemsDict = existingProblems.ToDictionary(p => (p.ContestId, p.Index));


        foreach (var dto in problems)
        {
            if (!contestsDict.TryGetValue(dto.ContestId, out var contestGuid))
            {
                continue;
            }

            statsDict.TryGetValue((dto.ContestId, dto.Index), out var stats);

            problemsDict.TryGetValue((dto.ContestId, dto.Index), out var problem);

            if (problem == null)
            {
                problem = _mapper.Map<Problem>(dto);
                problem.Id = Guid.NewGuid();
                await _unitOfWork.Problems.AddAsync(problem);
            }
            else
            {
                _mapper.Map(dto, problem);
            }

            problem.SolvedCount = stats?.SolvedCount ?? 0;
            problem.GuidContestId = contestGuid;

            problem.Tags ??= new List<Tag>();
            problem.Tags.Clear();
            if (dto.Tags != null)
            {
                foreach (var tagName in dto.Tags)
                {
                    if (tagsDict.TryGetValue(tagName, out var tagEntity))
                    {
                        problem.Tags.Add(tagEntity);
                    }
                }
            }

            var translation = problem.ProblemTranslations.FirstOrDefault(pt => pt.LanguageCode == languageCode);
            if (translation != null)
            {
                _mapper.Map(dto, translation);
            }
            else
            {
                translation = _mapper.Map<ProblemTranslation>(dto);
                translation.ProblemId = problem.Id;
                translation.LanguageCode = languageCode;
                problem.ProblemTranslations ??= new List<ProblemTranslation>();
                problem.ProblemTranslations.Add(translation);
            }
        }

        await _unitOfWork.SaveAsync();
    }

    public async Task PostContestsFromCodeforces(List<CodeforcesContest> contests, bool gym, string languageCode)
    {
        var contestIdsFromApi = contests.Select(c => c.ContestId).ToList();
        var existingContests = await _unitOfWork.Contests.GetByContestIdsAsync(contestIdsFromApi);
        var existingContestsDict = existingContests.ToDictionary(c => c.ContestId);

        var existingContestGuids = existingContests.Select(c => c.Id).ToList();
        var existingTranslations = await _unitOfWork.ContestTranslations.GetByContestIdsAndLanguageAsync(existingContestGuids, languageCode);
        var existingTranslationsDict = existingTranslations.ToDictionary(ct => ct.ContestId);

        List<Contest> contestsToUpsert = [];
        List<ContestTranslation> translationsToUpsert = [];

        foreach (var incomingContest in contests)
        {
            Guid contestGuid;
            Contest contestEntity;

            if (existingContestsDict.TryGetValue(incomingContest.ContestId, out var existingContest))
            {
                contestEntity = existingContest;
                contestGuid = existingContest.Id;
                _mapper.Map(incomingContest, contestEntity);
            }
            else
            {
                contestEntity = _mapper.Map<Contest>(incomingContest);
                contestGuid = Guid.NewGuid();
                contestEntity.Id = contestGuid;
                contestEntity.IsContestLoaded = false;
            }

            contestEntity.Gym = gym;
            contestsToUpsert.Add(contestEntity);

            ContestTranslation contestTranslationEntity;
            if (existingTranslationsDict.TryGetValue(contestGuid, out var existingTranslation))
            {
                contestTranslationEntity = existingTranslation;
                _mapper.Map(incomingContest, contestTranslationEntity);
            }
            else
            {
                contestTranslationEntity = _mapper.Map<ContestTranslation>(incomingContest);
                contestTranslationEntity.ContestId = contestGuid;
                contestTranslationEntity.LanguageCode = languageCode;
            }
            translationsToUpsert.Add(contestTranslationEntity);
        }

        await _unitOfWork.Contests.InsertOrUpdateAsync(contestsToUpsert);
        await _unitOfWork.ContestTranslations.InsertOrUpdateAsync(translationsToUpsert);
    }

    public async Task PostSubmissionsFromCodeforces(List<CodeforcesSubmission> submissions, string handle)
    {
        if (submissions == null || submissions.Count == 0)
        {
            return;
        }

        var submissionIdsFromApi = submissions.Select(s => s.Id).ToList();
        var existingSubmissions = await _unitOfWork.Submissions.GetBySubmissionIdsAsync(submissionIdsFromApi);
        var existingSubmissionsDict = existingSubmissions.ToDictionary(s => s.SubmissionId);

        var user = await _unitOfWork.Users.GetByHandleAsync(handle)
            ?? throw new Exception($"User {handle} not found");

        List<Submission> submissionsToUpsert = [];

        foreach (var incomingSubmission in submissions)
        {
            Submission submissionEntity;

            if (existingSubmissionsDict.TryGetValue(incomingSubmission.Id, out var existingSubmission))
            {
                submissionEntity = existingSubmission;
                _mapper.Map(incomingSubmission, submissionEntity);
            }
            else
            {
                submissionEntity = _mapper.Map<Submission>(incomingSubmission);
                submissionEntity.Id = Guid.NewGuid();
            }

            submissionEntity.UserId = user.Id;
            submissionsToUpsert.Add(submissionEntity);
        }

        await _unitOfWork.Submissions.InsertOrUpdateAsync(submissionsToUpsert);
    }

    public async Task PostRanklistRowsFromCodeforces(CodeforcesContestStanding contestStanding)
    {
        var contestId = contestStanding.Contest.ContestId;
        var problemIndexes = contestStanding.Problems.Select(p => p.Index).ToList();

        var existingRows = await _unitOfWork.RanklistRows.GetByContestIdAsync(contestId);
        var existingRowsDict = existingRows.ToDictionary(rr => (rr.Handle, rr.ParticipantType));

        var existingRowGuids = existingRows.Select(rr => rr.Id).ToList();
        var existingProblemResults = await _unitOfWork.ProblemResults.GetByRanklistRowIdsAsync(existingRowGuids);

        var existingProblemResultsDict = existingProblemResults
            .GroupBy(pr => pr.RanklistRowId)
            .ToDictionary(g => g.Key, g => g.ToDictionary(pr => pr.Index));

        List<RanklistRow> ranklistRowsToUpsert = [];
        List<ProblemResult> problemResultsToUpsert = [];

        foreach (var row in contestStanding.Rows)
        {
            var handle = row.Party.Members[0].Handle;
            Guid ranklistRowId;
            RanklistRow ranklistRowEntity;

            var rowKey = (handle, row.Party.ParticipantType);

            if (existingRowsDict.TryGetValue(rowKey, out var existingRow))
            {
                ranklistRowEntity = existingRow;
                ranklistRowId = existingRow.Id;
                _mapper.Map(row, ranklistRowEntity);
            }
            else
            {
                ranklistRowEntity = _mapper.Map<RanklistRow>(row);
                ranklistRowId = Guid.NewGuid();
                ranklistRowEntity.Id = ranklistRowId;
                ranklistRowEntity.ContestId = contestId;
            }
            ranklistRowsToUpsert.Add(ranklistRowEntity);

            for (int i = 0; i < row.ProblemResults.Count; i++)
            {
                var result = row.ProblemResults[i];
                var problemIndex = problemIndexes[i];
                ProblemResult problemResultEntity;

                if (existingProblemResultsDict.TryGetValue(ranklistRowId, out var resultsForThisRow) &&
                    resultsForThisRow.TryGetValue(problemIndex, out var existingResult))
                {
                    problemResultEntity = existingResult;
                    _mapper.Map(result, problemResultEntity);
                }
                else
                {
                    problemResultEntity = _mapper.Map<ProblemResult>(result);
                    problemResultEntity.Id = Guid.NewGuid();
                    problemResultEntity.RanklistRowId = ranklistRowId;
                    problemResultEntity.Index = problemIndex;
                }
                problemResultsToUpsert.Add(problemResultEntity);
            }
        }

        await _unitOfWork.RanklistRows.InsertOrUpdateAsync(ranklistRowsToUpsert);
        await _unitOfWork.ProblemResults.InsertOrUpdateAsync(problemResultsToUpsert);

        var contest = await _unitOfWork.Contests.GetByContestIdAsync(contestStanding.Contest.ContestId);
        if (contest!.Phase == "FINISHED")
        {
            contest.IsContestLoaded = true;
            _unitOfWork.Contests.Update(contest);
            await _unitOfWork.SaveAsync();
        }
    }
}