namespace Etrx.Core.Contracts.Submissions;

public record SubmissionsResponseDto(
    string Handle,
    string? FirstName,
    string? LastName,
    string? City,
    string? Organisation,
    int? Grade,
    int? SolvedCount,
    string? ParticipantType,
    List<int>? Tries
);