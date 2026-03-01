using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Domain.Expressions;
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

    public async Task PostUserFromDlCodeforcesAsync(DlUser dlUser, CodeforcesUser cfUser)
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

    public async Task PostProblemsFromCodeforcesAsync(List<CodeforcesProblem> problems, List<CodeforcesProblemStatistics> problemStatistics, string languageCode)
    {
        var identifiers = problems.Select(p => (p.ContestId, p.Index)).ToList();
        var existingProblems = await _unitOfWork.Problems.GetByContestAndIndexAsync(identifiers);
        var existingProblemsDict = existingProblems.ToDictionary(p => (p.ContestId, p.Index));

        var existingProblemGuids = existingProblems.Select(p => p.Id).ToList();
        var existingTranslations = await _unitOfWork.ProblemTranslations.GetByProblemIdsAndLanguageAsync(existingProblemGuids, languageCode);
        var existingTranslationsDict = existingTranslations.ToDictionary(pt => pt.ProblemId);

        var existingContests = await _unitOfWork.Contests.GetByContestIdsAsync(problems.Select(p => p.ContestId).ToList());
        var existingContestsDict = existingContests.ToDictionary(c => c.ContestId);

        var statisticsDict = problemStatistics.ToDictionary(s => (s.ContestId, s.Index));

        var allIncomingTagNames = problems.SelectMany(p => p.Tags).Distinct().ToList();
        var existingTags = await _unitOfWork.Tags.GetByNamesAsync(allIncomingTagNames);
        var tagsDict = existingTags.ToDictionary(t => t.Name);

        var newTags = allIncomingTagNames
            .Where(name => !tagsDict.ContainsKey(name))
            .Select(name => new Tag { Id = Guid.NewGuid(), Name = name })
            .ToList();

        if (newTags.Any())
        {
            foreach (var tag in newTags)
            {
                await _unitOfWork.Tags.AddAsync(tag);
                tagsDict[tag.Name] = tag;
            }
        }

        foreach (var incomingProblem in problems)
        {
            if (!existingContestsDict.TryGetValue(incomingProblem.ContestId, out var existingContest))
                continue;

            var problemKey = (incomingProblem.ContestId, incomingProblem.Index);
            statisticsDict.TryGetValue(problemKey, out var stats);

            Problem problemEntity;
            bool isNewProblem = false;

            if (existingProblemsDict.TryGetValue(problemKey, out var existingProblem))
            {
                problemEntity = existingProblem;
                _mapper.Map(incomingProblem, problemEntity);
            }
            else
            {
                isNewProblem = true;
                problemEntity = _mapper.Map<Problem>(incomingProblem);
                problemEntity.Id = Guid.NewGuid();
                await _unitOfWork.Problems.AddAsync(problemEntity);
            }

            problemEntity.SolvedCount = stats?.SolvedCount ?? 0;
            problemEntity.GuidContestId = existingContest.Id;

            problemEntity.Tags.Clear();
            foreach (var tagName in incomingProblem.Tags)
            {
                if (tagsDict.TryGetValue(tagName, out var tag))
                    problemEntity.Tags.Add(tag);
            }

            if (existingTranslationsDict.TryGetValue(problemEntity.Id, out var existingTranslation))
            {
                _mapper.Map(incomingProblem, existingTranslation);
            }
            else
            {
                var translation = _mapper.Map<ProblemTranslation>(incomingProblem);
                translation.ProblemId = problemEntity.Id;
                translation.LanguageCode = languageCode;
                await _unitOfWork.ProblemTranslations.AddAsync(translation);
            }
        }

        await _unitOfWork.SaveAsync();
    }

    public async Task PostContestsFromCodeforcesAsync(List<CodeforcesContest> contests, bool gym, string languageCode)
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

            contestEntity.Division = ContestExpressions.GetDivisionFromContestName(incomingContest.Name);

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

    public async Task PostSubmissionsFromCodeforcesAsync(List<CodeforcesSubmission> submissions, string handle)
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

    public async Task PostRanklistRowsFromCodeforcesAsync(CodeforcesContestStanding contestStanding)
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