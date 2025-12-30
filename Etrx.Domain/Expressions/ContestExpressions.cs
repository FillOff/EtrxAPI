using System.Text.RegularExpressions;

namespace Etrx.Domain.Expressions
{
    public static class ContestExpressions
    {
        public static string GetDivisionFromContestName(string? contestName)
        {
            if (string.IsNullOrWhiteSpace(contestName))
                return string.Empty;

            var match = Regex.Match(contestName, @"Div\.?\s*(\d+)", RegexOptions.IgnoreCase);

            if (match.Success)
                return $"Div{match.Groups[1].Value}";

            return "-";
        }
    }
}
