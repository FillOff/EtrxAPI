using Etrx.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etrx.Domain.Helpers;

public static class DivisionExpressions
{
    public static string GetDivisionName(int rating)
    {
        return rating switch
        {
            >= (int)Divisions.Div1 => nameof(Divisions.Div1),
            >= (int)Divisions.Div2 => nameof(Divisions.Div2),
            >= (int)Divisions.Div3 => nameof(Divisions.Div3),
            _ => nameof(Divisions.Div4)
        };
    }
}

