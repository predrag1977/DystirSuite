using Newtonsoft.Json;

namespace DystirManager.Models
{
    public class Status
    {
        [JsonProperty("StatusID")]
        public int StatusID { get; set; }

        [JsonProperty("StatusName")]
        public string StatusName { get; set; }
    }
}