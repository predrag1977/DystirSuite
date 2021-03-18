using Newtonsoft.Json;

namespace DystirXamarin.Models
{
    public class Status
    {
        [JsonProperty("StatusID")]
        public int StatusID { get; set; }

        [JsonProperty("StatusName")]
        public string StatusName { get; set; }
    }
}