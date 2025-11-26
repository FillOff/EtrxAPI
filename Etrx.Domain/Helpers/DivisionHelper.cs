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
        public static Divisions GetDivisionByRating(int rating)
        {
            if (rating >= 2100) return Divisions.Div1;
            if (rating >= 1600) return Divisions.Div2;
            if (rating >= 1400) return Divisions.Div3;
            if (rating >= 0) return Divisions.Div4;
            return Divisions.Unknown;
        }

        public static List<Divisions> GetAllDivisions()
        {
            return Enum.GetValues(typeof(Divisions))
                       .Cast<Divisions>()
                       .Where(d => d != Divisions.Unknown)
                       .ToList();
        }

        public static string GetDivisionName(int rating)
        {
            if (rating >= 2100) return nameof(Divisions.Div1);
            if (rating >= 1600) return nameof(Divisions.Div2);
            if (rating >= 1400) return nameof(Divisions.Div3);
            return nameof(Divisions.Div4);
        }
    }
}

