namespace Etrx.Application.Dtos.Submissions;

// One accepted user/problem/type combination, read from the stored protocol.
public record ProtocolResultFactDto(
    string Handle, string FirstName, string LastName, int UserRating,
    int ContestId, string Index, string ParticipantType,
    string? Division, int? ProblemRating);
