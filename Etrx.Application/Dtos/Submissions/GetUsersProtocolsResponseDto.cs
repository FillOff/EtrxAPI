namespace Etrx.Application.Dtos.Submissions;

public class GetUsersProtocolsResponseDto
{
    public string Handle { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int ContestsCount { get; set; }
    public int SolvedCount { get; set; }
    public long LastTime { get; set; }
}