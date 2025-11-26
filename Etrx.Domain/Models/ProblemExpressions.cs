using Etrx.Domain.Entities;
using System.Linq.Expressions;

namespace Etrx.Domain.Models;

public static class ProblemExpressions
{
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Expression<Func<Problem, int>> DifficultyExpression =>
        p => p.SolvedCount <= 0
            ? 100
            : (int)Math.Round(
                (DateTime.UtcNow - UnixEpoch.AddSeconds(p.Contest.StartTime)).TotalDays < 1
                    ? 1.0
                    : Math.Max(1.0, (DateTime.UtcNow - UnixEpoch.AddSeconds(p.Contest.StartTime)).TotalDays / p.SolvedCount)
              );
}