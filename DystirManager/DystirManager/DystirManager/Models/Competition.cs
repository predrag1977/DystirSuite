using Newtonsoft.Json;

namespace DystirManager.Models
{
    public class MatchType
    {
        [JsonProperty("MatchTypeID")]
        public int MatchTypeID { get; set; }

        [JsonProperty("MatchTypeName")]
        public string MatchTypeName { get; internal set; }
    }
}