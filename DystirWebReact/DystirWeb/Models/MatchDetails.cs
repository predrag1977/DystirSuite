using System.Runtime.Serialization;

namespace DystirWeb.Shared
{
    [DataContract]
    public class MatchDetails
    {
        [DataMember]
        public int MatchDetailsID { get; set; }
        [DataMember]
        public Matches Match { get; set; }
        [DataMember]
        public List<EventsOfMatches> EventsOfMatch { get; set; } = new List<EventsOfMatches>();
        [DataMember]
        public List<PlayersOfMatches> PlayersOfMatch { get; set; } = new List<PlayersOfMatches>();
        [DataMember]
        public List<Standing> Standings { get; set; } = new List<Standing>();
        [DataMember]
        public Statistic Statistic { get; set; } = new Statistic();
    }
}