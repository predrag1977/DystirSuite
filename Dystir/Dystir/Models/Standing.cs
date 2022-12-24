using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dystir.Models
{
    [DataContract]
    public class Standing
    {
        [DataMember]
        public string StandingCompetitionName { get; set; }
        [DataMember]
        public IEnumerable<TeamStanding> TeamStandings { get; set; }
    }
}