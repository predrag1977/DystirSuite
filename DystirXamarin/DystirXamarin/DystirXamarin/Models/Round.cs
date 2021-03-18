using Newtonsoft.Json;

namespace DystirXamarin.Models
{
    public class Round
    {
        [JsonProperty("RoundID")]
        public int? RoundID { get; set; }

        [JsonProperty("RoundName")]
        public string RoundName { get; set; }
    }
}