using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Interfaces;

public interface ICodeforcesProgressClient
{
    Task<Dictionary<(int ContestId, string Index), ProgressFacts>> GetHistoryAsync(string handle, CancellationToken cancellationToken);
}
