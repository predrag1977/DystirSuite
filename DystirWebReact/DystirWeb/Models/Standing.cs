using System.Runtime.Serialization;

namespace DystirWeb.Models
{
    [DataContract]
    public class Standing
    {
        [DataMember]
        public string StandingCompetitionId { get; set; }

        [DataMember]
        public int StandingTypeID { get; internal set; }

        [DataMember]
        public string StandingCompetitionName { get; set; }

        [DataMember]
        public IEnumerable<TeamStanding> TeamStandings { get; set; }

    }
}

