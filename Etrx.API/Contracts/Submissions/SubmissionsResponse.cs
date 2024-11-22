namespace Etrx.API.Contracts.Submissions
{
    public record SubmissionsResponse(
        string Handle,
        string? FirstName,
        string? LastName,
        string? City,
        string? Organisation,
        int? Grade,
        int? SolvedCount,
        string? ParticipantType,
        int[]? Tries
    );
}