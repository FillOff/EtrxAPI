using Etrx.Domain.Models;

namespace Etrx.Application.Services;

public static class RanklistPartyFormatter
{
    public static (string Organization, string City) GetOrganizationAndCity(RanklistRow row, User primaryUser)
    {
        return row.MemberHandles.Count > 1
            ? (string.Empty, string.Empty)
            : (primaryUser.Organization, primaryUser.City);
    }

    public static string FormatName(string teamName, IReadOnlyCollection<string> memberHandles, IEnumerable<User> users)
    {
        var usersByHandle = users.ToDictionary(user => user.Handle, StringComparer.OrdinalIgnoreCase);
        var memberNames = memberHandles
            .Select(handle => usersByHandle.TryGetValue(handle, out var user)
                ? string.Join(" ", new[] { user.LastName, user.FirstName }.Where(name => !string.IsNullOrWhiteSpace(name)))
                : handle)
            .ToList();

        if (memberNames.Count <= 1 || string.IsNullOrWhiteSpace(teamName))
        {
            return memberNames.FirstOrDefault() ?? teamName;
        }

        return $"{teamName} ({string.Join(", ", memberNames)})";
    }
}