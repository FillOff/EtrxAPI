using Etrx.Application.Constants;
using Etrx.Application.Dtos.Submissions;

namespace Etrx.Application.Services;

public static class ProtocolResultsCalculator
{
    public static readonly string[] Divisions = ["ALL", "Div1", "Div2", "Div3", "Div4"];
    public static readonly string[] Types =
    [ParticipantTypes.Contestant, ParticipantTypes.Virtual,
        ParticipantTypes.OutOfCompetition, ParticipantTypes.Practice];

    public static List<DivisionResultResponseDto> Results(
        IEnumerable<ProtocolResultFactDto> facts, string division, string participantType)
    {
        return Filter(facts, division, participantType)
            .GroupBy(f => f.Handle)
            .Select(group =>
            {
                var user = group.First();
                var problems = group.DistinctBy(f => (f.ContestId, f.Index)).ToList();
                var ratings = problems.Where(f => f.ProblemRating > 0)
                    .Select(f => f.ProblemRating!.Value).ToList();
                return new DivisionResultResponseDto
                {
                    Handle = user.Handle,
                    Name = $"{user.LastName} {user.FirstName}".Trim(),
                    Rating = user.UserRating,
                    SolvedCount = problems.Count,
                    RatedProblemCount = ratings.Count,
                    // Keep full precision for sorting; round only in the UI.
                    AverageProblemRating = ratings.Count == 0 ? 0 : ratings.Average()
                };
            })
            .OrderByDescending(r => r.SolvedCount)
            .ThenByDescending(r => r.AverageProblemRating)
            .ThenByDescending(r => r.Rating)
            .ThenBy(r => r.Handle, StringComparer.Ordinal)
            .ToList();
    }


    public static List<ProtocolUserMatrixResponseDto> UserMatrix(IEnumerable<ProtocolResultFactDto> facts)
    {
        var rows = facts.Where(f => Types.Contains(f.ParticipantType)).ToList();
        return rows.GroupBy(f => f.Handle).Select(group =>
        {
            var user = group.First();
            int Count(string division) => group
                .Where(f => division == "ALL" || string.Equals(f.Division?.Trim(), division, StringComparison.OrdinalIgnoreCase))
                .DistinctBy(f => (f.ContestId, f.Index)).Count();
            return new ProtocolUserMatrixResponseDto
            {
                Handle = user.Handle,
                Name = $"{user.LastName} {user.FirstName}".Trim(),
                Rating = user.UserRating,
                All = Count("ALL"), Div1 = Count("Div1"), Div2 = Count("Div2"),
                Div3 = Count("Div3"), Div4 = Count("Div4")
            };
        })
        .OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase).ThenBy(r => r.Handle, StringComparer.Ordinal).ToList();
    }

    public static List<ProtocolUserTypeMatrixResponseDto> UserTypeMatrix(
        IEnumerable<ProtocolResultFactDto> facts, string division)
    {
        var rows = Filter(facts, division, ParticipantTypes.All).ToList();
        return rows.GroupBy(f => f.Handle).Select(group =>
        {
            var user = group.First();
            int Count(string type) => group.Where(f => f.ParticipantType == type)
                .DistinctBy(f => (f.ContestId, f.Index)).Count();
            var contestant = Count(ParticipantTypes.Contestant);
            var virtualCount = Count(ParticipantTypes.Virtual);
            var outCount = Count(ParticipantTypes.OutOfCompetition);
            var practice = Count(ParticipantTypes.Practice);
            return new ProtocolUserTypeMatrixResponseDto
            {
                Handle = user.Handle, Name = $"{user.LastName} {user.FirstName}".Trim(),
                Rating = user.UserRating, Contestant = contestant, Virtual = virtualCount,
                OutOfCompetition = outCount, Practice = practice,
                Total = group.DistinctBy(f => (f.ContestId, f.Index)).Count()
            };
        })
        .OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase).ThenBy(r => r.Handle, StringComparer.Ordinal).ToList();
    }

    public static List<ProtocolTaskDetailResponseDto> Tasks(
        IEnumerable<ProtocolResultFactDto> facts, string handle, string division, string participantType)
    {
        return Filter(facts, division, participantType)
            .Where(f => string.Equals(f.Handle, handle, StringComparison.OrdinalIgnoreCase))
            .DistinctBy(f => (f.ContestId, f.Index))
            .OrderBy(f => f.ContestId)
            .ThenBy(f => f.Index, StringComparer.OrdinalIgnoreCase)
            .Select(f => new ProtocolTaskDetailResponseDto
            {
                Handle = f.Handle,
                ContestId = f.ContestId,
                Index = f.Index,
                ProblemRating = f.ProblemRating
            })
            .ToList();
    }

    public static List<ProtocolSummaryResponseDto> Summary(IReadOnlyList<ProtocolResultFactDto> facts)
    {
        return Types.SelectMany(type => Divisions.Select(division =>
        {
            var filtered = Filter(facts, division, type).ToList();
            return new ProtocolSummaryResponseDto(type, division,
                filtered.DistinctBy(f => (f.Handle, f.ContestId, f.Index)).Count(),
                filtered.Select(f => f.Handle).Distinct().Count());
        })).ToList();
    }

    private static IEnumerable<ProtocolResultFactDto> Filter(
        IEnumerable<ProtocolResultFactDto> facts, string division, string type) =>
        facts.Where(f => Types.Contains(f.ParticipantType)
            && (type == ParticipantTypes.All || f.ParticipantType == type)
            && (division == "ALL" || string.Equals(f.Division?.Trim(), division,
                StringComparison.OrdinalIgnoreCase)));
}
