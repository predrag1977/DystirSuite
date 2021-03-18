using Newtonsoft.Json;
using System;

namespace Dystir.Models
{
    public class Team
    {
        [JsonProperty("TeamID")]
        public int TeamID { get; set; }

        [JsonProperty("TeamName")]
        public string TeamName { get; internal set; }

        [JsonProperty("TeamLocation")]
        public string TeamLocation { get; internal set; }
    }
}