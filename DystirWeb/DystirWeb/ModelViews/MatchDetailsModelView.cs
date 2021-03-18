using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DystirWeb.Models;

namespace DystirWeb.ModelViews
{
    [DataContract]
    public class MatchDetails
    {
        [DataMember]
        public Matches Match { get; internal set; }
        [DataMember]
        public List<EventsOfMatches> EventsOfMatch { get; internal set; } = new List<EventsOfMatches>();
        [DataMember]
        public List<PlayersOfMatches> PlayersOfMatch { get; internal set; } = new List<PlayersOfMatches>();
    }
}