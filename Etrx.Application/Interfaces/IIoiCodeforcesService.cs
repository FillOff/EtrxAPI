using Etrx.Domain.Models.Parsing_models.IoiCodeforces;

namespace Etrx.Application.Interfaces;

public interface IIoiCodeforcesService
{
    Task PostContestsAsync(List<IoiCodeforcesContest> contests, string languageCode);
    Task PostRanklistRowsAsync(int contestId, IoiCodeforcesStandings standings);
}
