using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Codeforces
{
    public class CodeforcesUser
    {
        [JsonProperty("handle")]
        public string Handle { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("vkId")]
        public string VkId { get; set; } = string.Empty;

        [JsonProperty("openId")]
        public string OpenId { get; set; } = string.Empty;

        [JsonProperty("country")]
        public string Country { get; set; } = string.Empty;

        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;

        [JsonProperty("contribution")]
        public int Contribution { get; set; } = 0;

        [JsonProperty("rank")]
        public string Rank { get; set; } = string.Empty;

        [JsonProperty("rating")]
        public int Rating { get; set; } = 0;

        [JsonProperty("maxRank")]
        public string MaxRank { get; set; } = string.Empty;

        [JsonProperty("maxRating")]
        public int MaxRating { get; set; } = 0;

        [JsonProperty("lastOnlineTimeSeconds")]
        public long LastOnlineTimeSeconds { get; set; } = 0;

        [JsonProperty("registrationTimeSeconds")]
        public long RegistrationTimeSeconds { get; set; } = 0;

        [JsonProperty("friendOfCount")]
        public int FriendOfCount { get; set; } = 0;

        [JsonProperty("avatar")]
        public string Avatar { get; set; } = string.Empty;

        [JsonProperty("titlePhoto")]
        public string TitlePhoto { get; set; } = string.Empty;
    }
}
