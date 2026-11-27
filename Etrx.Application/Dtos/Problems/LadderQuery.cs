using System.ComponentModel.DataAnnotations;
using Etrx.Application.Constants;

namespace Etrx.Application.Dtos.Problems;

public sealed class LadderQuery
{
    [Required] public string Division { get; init; } = string.Empty;
    [Range(1, int.MaxValue)] public int? Difficulty { get; init; }
    [Range(0, int.MaxValue)] public int? Rating { get; init; }
    public string[] Tags { get; init; } = [];
    public string Lang { get; init; } = Languages.Ru;
    [Range(0, int.MaxValue)] public int GroupOffset { get; init; }
    [Range(1, 100)] public int? GroupLimit { get; init; }
    public bool IncludeProblems { get; init; } = true;
}

public record LadderLevel(string Division, int Difficulty);
public record LadderTag(string Name, int Priority);
public record LadderProblem(int ContestId, string Index, string Name, int Rating, int SolvedCount, bool Gym, string Source,
    bool IsSolved = false, string Status = ProblemStatuses.None);
public record LadderGroup(string[] Tags, LadderProblem[] Problems, int ProblemCount, int SolvedCount = 0, int FailedCount = 0);
public record LadderResponse(LadderTag[] AvailableTags, LadderGroup[] Groups, int TotalGroups);
public record LadderStatistic(string Handle, int ContestId, string Index, int Attempts,
    bool Solved, long? FirstAcceptedAt, long LastAttemptAt);

public record LadderParticipantReport(string Division, int? Difficulty, int? Rating,
    string[] Tags, bool ExactTags, int TotalProblems, LadderParticipantResult[] Participants);
public record LadderParticipantResult(string Handle, string FirstName, string LastName,
    int SolvedCount, LadderSolvedProblem[] SolvedProblems);
public record LadderSolvedProblem(int ContestId, string Index, string Name, int Rating,
    bool Gym, string Source, long FirstAcceptedAt);
