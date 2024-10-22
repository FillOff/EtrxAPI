using Newtonsoft.Json;

namespace Etrx.Domain.Parsing_models
{
    public class CodeforcesUser
    {
        [JsonProperty("handle")]
        public string Handle { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("vkId")]
        public string? VkId { get; set; }

        [JsonProperty("openId")]
        public string? OpenId { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("contribution")]
        public int? Contribution { get; set; }

        [JsonProperty("rank")]
        public string? Rank { get; set; }

        [JsonProperty("rating")]
        public int? Rating { get; set; }

        [JsonProperty("maxRank")]
        public string? MaxRank { get; set; }

        [JsonProperty("maxRating")]
        public int? MaxRating { get; set; }

        [JsonProperty("lastOnlineTimeSeconds")]
        public long? LastOnlineTimeSeconds { get; set; }

        [JsonProperty("registrationTimeSeconds")]
        public long? RegistrationTimeSeconds { get; set; }

        [JsonProperty("friendOfCount")]
        public int? FriendOfCount { get; set; }

        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        [JsonProperty("titlePhoto")]
        public string? TitlePhoto { get; set; }
    }
}
