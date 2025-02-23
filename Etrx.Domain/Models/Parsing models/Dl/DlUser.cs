using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Dl
{
    public class DlUser
    {
        [JsonProperty("nick_name")]
        public string Handle { get; set; } = string.Empty;

        [JsonProperty("first_name")]
        public string? FirstName { get; set; }

        [JsonProperty("last_name")]
        public string? LastName { get; set; }

        [JsonProperty("grade")]
        public int? Grade { get; set; }

        [JsonProperty("school_name")]
        public string? Organization { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }
    }
}
