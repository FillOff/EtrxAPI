using Etrx.Domain.Enums;
using Etrx.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Etrx.Domain.Expressions;

public static class DivisionExpressions
{
    public static DivisionsEnum GetDivision(int? rating)
    {
        if (!rating.HasValue) return DivisionsEnum.Div4;
        int r = rating.Value;

        if (r >= (int)DivisionsEnum.Div1) return DivisionsEnum.Div1;
        if (r >= (int)DivisionsEnum.Div2) return DivisionsEnum.Div2;
        if (r >= (int)DivisionsEnum.Div3) return DivisionsEnum.Div3;
        return DivisionsEnum.Div4;
    }
    public static Expression<Func<Problem, bool>> GetPredicate(List<DivisionsEnum> divisions)
    {
        return p =>
            (divisions.Contains(DivisionsEnum.Div1) && p.Rating >= (int)DivisionsEnum.Div1) ||
            (divisions.Contains(DivisionsEnum.Div2) && p.Rating >= (int)DivisionsEnum.Div2 && p.Rating < (int)DivisionsEnum.Div1) ||
            (divisions.Contains(DivisionsEnum.Div3) && p.Rating >= (int)DivisionsEnum.Div3 && p.Rating < (int)DivisionsEnum.Div2) ||
            (divisions.Contains(DivisionsEnum.Div4) && p.Rating < (int)DivisionsEnum.Div3);
    }
}

