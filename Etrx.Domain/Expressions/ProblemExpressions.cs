using Etrx.Domain.Enums;
using Etrx.Domain.Models;
using System.Linq.Expressions;

namespace Etrx.Domain.Expressions;

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

    public static RankEnum GetRank(int? rating) =>
        rating == null ? RankEnum.Rank4 :
        rating >= (int)RankEnum.Rank1 ? RankEnum.Rank1 :
        rating >= (int)RankEnum.Rank2 ? RankEnum.Rank2 :
        rating >= (int)RankEnum.Rank3 ? RankEnum.Rank3 :
        RankEnum.Rank4;

    public static Expression<Func<Problem, bool>> GetPredicate(List<RankEnum> ranks)
    {
        return p =>
            (ranks.Contains(RankEnum.Rank1) && p.Rating >= (int)RankEnum.Rank1) ||
            (ranks.Contains(RankEnum.Rank2) && p.Rating >= (int)RankEnum.Rank2 && p.Rating < (int)RankEnum.Rank1) ||
            (ranks.Contains(RankEnum.Rank3) && p.Rating >= (int)RankEnum.Rank3 && p.Rating < (int)RankEnum.Rank2) ||
            (ranks.Contains(RankEnum.Rank4) && p.Rating < (int)RankEnum.Rank3);
    }
}