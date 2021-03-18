using Newtonsoft.Json;

namespace DystirXamarin.Models
{
    public class Squad
    {
        [JsonProperty("SquadID")]
        public int SquadID { get; set; }

        [JsonProperty("SquadName")]
        public string SquadName { get; internal set; }

        [JsonProperty("SquadShortName")]
        public string SquadShortName { get; internal set; }
    }
}