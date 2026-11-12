namespace Etrx.Domain.Models;

public class UserProblemPreferences
{
    public Guid UserId { get; set; }
    public string FiltersJson { get; set; } = "{}";
}
