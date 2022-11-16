using Newtonsoft.Json;

namespace Dystir.Models
{
    public class MatchType
    {
        [JsonProperty("MatchTypeID")]
        public int MatchTypeID { get; set; }

        [JsonProperty("MatchTypeName")]
        public string MatchTypeName { get; internal set; }
    }
}