namespace Etrx.Application.Queries;

public record HandleContestProtocolQueryParameters(
    string Handle,
    int ContestId,
    long UnixFrom,
    long UnixTo);