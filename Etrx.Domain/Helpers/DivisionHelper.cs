using Etrx.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Etrx.Domain.Enums;

namespace Etrx.Domain.Helpers
{
    public static class DivisionHelper
    {
        public static string GetDivisionName(int rating)
        {
            if (rating >= 2100) return nameof(Divisions.Div1);
            if (rating >= 1600) return nameof(Divisions.Div2);
            if (rating >= 1400) return nameof(Divisions.Div3);
            return nameof(Divisions.Div4);
        }
    }
}

