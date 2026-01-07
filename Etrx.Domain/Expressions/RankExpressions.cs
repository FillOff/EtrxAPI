using Etrx.Domain.Enums;
using Etrx.Domain.Models;
using System.Linq.Expressions;

namespace Etrx.Domain.Expressions;

public static class RankExpressions
{
    public static RankEnum GetRank(int? rating)
    {
        if (!rating.HasValue) return RankEnum.Rank4;
        int r = rating.Value;

        if (r >= (int)RankEnum.Rank1) return RankEnum.Rank1;
        if (r >= (int)RankEnum.Rank2) return RankEnum.Rank2;
        if (r >= (int)RankEnum.Rank3) return RankEnum.Rank3;
        return RankEnum.Rank4;
    }
    public static Expression<Func<Problem, bool>> GetPredicate(List<RankEnum> ranks)
    {
        return p =>
            (ranks.Contains(RankEnum.Rank1) && p.Rating >= (int)RankEnum.Rank1) ||
            (ranks.Contains(RankEnum.Rank2) && p.Rating >= (int)RankEnum.Rank2 && p.Rating < (int)RankEnum.Rank1) ||
            (ranks.Contains(RankEnum.Rank3) && p.Rating >= (int)RankEnum.Rank3 && p.Rating < (int)RankEnum.Rank2) ||
            (ranks.Contains(RankEnum.Rank4) && p.Rating < (int)RankEnum.Rank3);
    }
}

