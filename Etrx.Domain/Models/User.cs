namespace Etrx.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Handle { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? VkId { get; set; }
    public string? OpenId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Organization { get; set; }
    public int? Contribution { get; set; }
    public string? Rank { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string? MaxRank { get; set; } = string.Empty;
    public int? MaxRating { get; set; }
    public long? LastOnlineTimeSeconds { get; set; }
    public long? RegistrationTimeSeconds { get; set; }
    public int? FriendOfCount { get; set; }
    public string? Avatar { get; set; } = string.Empty;
    public string? TitlePhoto { get; set; } = string.Empty;
    public int? Grade { get; set; }
}