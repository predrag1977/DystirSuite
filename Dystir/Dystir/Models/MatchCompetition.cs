using Dystir.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dystir.Models
{
    public class MatchCompetition
    {
        [JsonProperty("Id")]
        public int? ID { get; internal set; }

        [JsonProperty("MatchTypeId")]
        public int? MatchTypeID { get; internal set; }

        [JsonProperty("MatchTypeName")]
        public string MatchTypeName { get; internal set; }

        [JsonProperty("CompetitionID")]
        public int? CompetitionID { get; internal set; }

        [JsonProperty("OrderID")]
        public int? OrderID { get; internal set; }
    }
}
	

