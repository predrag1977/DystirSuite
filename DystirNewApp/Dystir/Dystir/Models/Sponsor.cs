using Dystir.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
    }
}