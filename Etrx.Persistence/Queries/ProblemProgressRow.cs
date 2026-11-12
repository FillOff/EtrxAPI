using Etrx.Application.Constants;

namespace Etrx.Persistence.Queries;

public sealed class ProblemProgressRow
{
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;
    public string Status { get; set; } = ProblemStatuses.None;
    public bool IsDeferred { get; set; }
}
