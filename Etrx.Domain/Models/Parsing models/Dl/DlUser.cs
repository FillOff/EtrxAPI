using Newtonsoft.Json;

namespace Etrx.Domain.Models.ParsingModels.Dl
{
    public class DlUser
    {
        [JsonProperty("nick_name")]
        public string Handle { get; set; } = string.Empty;

        [JsonProperty("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("grade")]
        public int Grade { get; set; } = 0;

        [JsonProperty("school_name")]
        public string Organization { get; set; } = string.Empty;

        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;
    }
}
