using Newtonsoft.Json;

namespace DystirManager.Models
{
    public class Player
    {
        [JsonProperty("PlayerID")]
        public int PlayerID { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; internal set; }

        [JsonProperty("LastName")]
        public string LastName { get; internal set; }

        [JsonProperty("Team")]
        public string Team { get; internal set; }
    }
}