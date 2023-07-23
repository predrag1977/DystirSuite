using Newtonsoft.Json;
using Xamarin.Forms;

namespace Dystir.Models
{
    public class Sponsor
    {
        [JsonProperty("SponsorID")]
        public int SponsorID { get; internal set; }

        [JsonProperty("SponsorsName")]
        public string SponsorsName { get; internal set; }

        [JsonProperty("SponsorWebSite")]
        public string SponsorWebSite { get; internal set; }

        public Size Size { get; set; }
    }
}