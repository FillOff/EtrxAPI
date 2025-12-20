using Etrx.Domain.Enums;
using Etrx.Domain.Models;
using System.Linq.Expressions;

namespace Etrx.Domain.Expressions;

public static class RankExpressions
{
    public static RanksEnum GetRank(int? rating)
    {
        if (!rating.HasValue) return RanksEnum.Rank4;
        int r = rating.Value;

        if (r >= (int)RanksEnum.Rank1) return RanksEnum.Rank1;
        if (r >= (int)RanksEnum.Rank2) return RanksEnum.Rank2;
        if (r >= (int)RanksEnum.Rank3) return RanksEnum.Rank3;
        return RanksEnum.Rank4;
    }
    public static Expression<Func<Problem, bool>> GetPredicate(List<RanksEnum> ranks)
    {
        return p =>
            (ranks.Contains(RanksEnum.Rank1) && p.Rating >= (int)RanksEnum.Rank1) ||
            (ranks.Contains(RanksEnum.Rank2) && p.Rating >= (int)RanksEnum.Rank2 && p.Rating < (int)RanksEnum.Rank1) ||
            (ranks.Contains(RanksEnum.Rank3) && p.Rating >= (int)RanksEnum.Rank3 && p.Rating < (int)RanksEnum.Rank2) ||
            (ranks.Contains(RanksEnum.Rank4) && p.Rating < (int)RanksEnum.Rank3);
    }
}

