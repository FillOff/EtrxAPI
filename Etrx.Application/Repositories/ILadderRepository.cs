using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Repositories;

public interface ILadderRepository
{
    Task<List<LadderLevel>> GetLevelsAsync(CancellationToken cancellationToken);
    Task<List<int>> GetRatingsAsync(LadderQuery query, CancellationToken cancellationToken);
    Task<List<int>> GetDifficultiesAsync(LadderQuery query, CancellationToken cancellationToken);
    Task<LadderResponse> GetLadderAsync(LadderQuery query, CancellationToken cancellationToken, Guid? userId = null);
    Task<List<LadderStatistic>> GetStatisticsAsync(LadderQuery query, CancellationToken cancellationToken);
    Task<LadderParticipantReport> GetParticipantsAsync(LadderQuery query, bool exactTags, CancellationToken cancellationToken);
}
