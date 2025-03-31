namespace Etrx.Core.Contracts.Submissions;

public record class GetSubmissionRequestDto(
    string SortField = "solvedCount",
    bool SortOrder = true,
    bool AllIndexes = true,
    string FilterByParticipantType = "ALL");