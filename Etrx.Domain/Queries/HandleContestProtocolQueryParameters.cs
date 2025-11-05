namespace Etrx.Domain.Queries;

public record HandleContestProtocolQueryParameters(
    string Handle,
    int ContestId,
    long UnixFrom,
    long UnixTo);