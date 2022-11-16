using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dystir.Models
{
    //public class Standing
    //{
    //    //[JsonProperty("StandingCompetitionName")]
    //    //public string StandingCompetitionName { get; set; }

    //    //[JsonProperty("TeamStandings")]
    //    //public IEnumerable<TeamStanding> TeamStandings { get; internal set; }

        

    //}

    [DataContract]
    public class Standing
    {
        [DataMember]
        public string StandingCompetitionName { get; set; }
        [DataMember]
        public IEnumerable<TeamStanding> TeamStandings { get; set; }
    }
}