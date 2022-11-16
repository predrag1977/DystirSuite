using Newtonsoft.Json;

namespace Dystir.Models
{
    public class Status
    {
        [JsonProperty("StatusID")]
        public int StatusID { get; set; }

        [JsonProperty("StatusName")]
        public string StatusName { get; set; }
    }
}