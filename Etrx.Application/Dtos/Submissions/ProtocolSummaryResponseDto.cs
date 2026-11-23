namespace Etrx.Application.Dtos.Submissions;

public record ProtocolSummaryResponseDto(
    string ParticipantType, string Division, int SolvedCount, int ParticipantCount);
