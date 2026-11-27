using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController, AllowAnonymous, Route("api/ladder")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class LadderController(ILadderRepository ladder, IAuthSession session) : ControllerBase
{
    /// <summary>Available division and difficulty pairs in the problem catalogue.</summary>
    [HttpGet("levels")]
    public Task<List<LadderLevel>> GetLevels(CancellationToken cancellationToken) =>
        ladder.GetLevelsAsync(cancellationToken);

    /// <summary>Ratings present in the selected division, in ascending order.</summary>
    [HttpGet("ratings")]
    public Task<List<int>> GetRatings([FromQuery] LadderQuery query, CancellationToken cancellationToken) =>
        ladder.GetRatingsAsync(query, cancellationToken);

    /// <summary>Difficulties present in the selected division and optional rating, in ascending order.</summary>
    [HttpGet("difficulties")]
    public Task<List<int>> GetDifficulties([FromQuery] LadderQuery query, CancellationToken cancellationToken) =>
        ladder.GetDifficultiesAsync(query, cancellationToken);

    /// <summary>Groups ordered by tag priority. Repeat tags to include their exact subsets. Omit tags for the entire level.</summary>
    [HttpGet]
    public Task<LadderResponse> Get([FromQuery] LadderQuery query, CancellationToken cancellationToken) =>
        ladder.GetLadderAsync(query, cancellationToken, session.UserId);

    /// <summary>Imported attempts per handle and problem in the same selection. Missing rows mean zero recorded attempts.
    /// Join by contestId and index. Timestamps are Unix seconds in UTC.</summary>
    [HttpGet("statistics")]
    public Task<List<LadderStatistic>> GetStatistics([FromQuery] LadderQuery query, CancellationToken cancellationToken) =>
        ladder.GetStatisticsAsync(query, cancellationToken);

    /// <summary>Participants, names and distinct solved problems in the selection.
    /// exactTags=true selects a single branch with exactly these tags, including the untagged branch.</summary>
    [HttpGet("participants")]
    public Task<LadderParticipantReport> GetParticipants([FromQuery] LadderQuery query,
        [FromQuery] bool exactTags, CancellationToken cancellationToken) =>
        ladder.GetParticipantsAsync(query, exactTags, cancellationToken);
}
