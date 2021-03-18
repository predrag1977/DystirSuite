using Newtonsoft.Json;

namespace Dystir.Models
{
    public class Round
    {
        [JsonProperty("RoundID")]
        public int? RoundID { get; set; }

        [JsonProperty("RoundName")]
        public string RoundName { get; set; }
    }
}