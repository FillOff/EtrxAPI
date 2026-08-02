namespace Etrx.Domain.Models.Parsing_models.IoiCodeforces;

public class IoiCodeforcesStandings
{
    public string ContestName { get; set; } = string.Empty;
    public List<IoiCodeforcesParticipantRow> Rows { get; set; } = new();

}
