namespace Etrx.Domain.Models;

public class User
{
    public User(
        int id,
        string handle,
        string? email,
        string? vkId,
        string? openId,
        string? firstName,
        string? lastName,
        string? country,
        string? city,
        string? organization,
        int? contribution,

        string? rank,
        int? rating,
        string? maxRank,
        int? maxRating,

        long? lastOnlineTimeSeconds,
        long? registrationTimeSeconds,
        int? friendOfCount,
        string? avatar,
        string? titlePhoto,
        int? grade,
        int? dlId,
        bool? watch)
    {
        Id = id;
        Handle = handle;
        Email = email;
        VkId = vkId;
        OpenId = openId;
        FirstName = firstName;
        LastName = lastName;
        Country = country;
        City = city;
        Organization = organization;
        Contribution = contribution;
        Rank = rank;
        Rating = rating;
        MaxRank = maxRank;
        MaxRating = maxRating;
        LastOnlineTimeSeconds = lastOnlineTimeSeconds;
        RegistrationTimeSeconds = registrationTimeSeconds;
        FriendOfCount = friendOfCount;
        Avatar = avatar;
        TitlePhoto = titlePhoto;
        Grade = grade;
        DlId = dlId;
        Watch = watch;
    }

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

    public int? DlId { get; set; } = null;

    public bool? Watch { get; set; }
}