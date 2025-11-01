namespace Etrx.Domain.Dtos.Submissions;

public record class SubmissionsResponseDto(
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