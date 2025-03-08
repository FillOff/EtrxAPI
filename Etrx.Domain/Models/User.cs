namespace Etrx.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Handle { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string VkId { get; set; } = string.Empty;
    public string OpenId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public int Contribution { get; set; } = 0;
    public string Rank { get; set; } = string.Empty;
    public int Rating { get; set; } = 0;
    public string MaxRank { get; set; } = string.Empty;
    public int MaxRating { get; set; } = 0;
    public long LastOnlineTimeSeconds { get; set; } = 0;
    public long RegistrationTimeSeconds { get; set; } = 0;
    public int FriendOfCount { get; set; } = 0;
    public string Avatar { get; set; } = string.Empty;
    public string TitlePhoto { get; set; } = string.Empty;
    public int Grade { get; set; } = 0;
}