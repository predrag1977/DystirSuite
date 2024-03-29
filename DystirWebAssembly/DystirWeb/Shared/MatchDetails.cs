﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DystirWeb.Shared
{
    [DataContract]
    public class MatchDetails
    {
        [DataMember]
        public Matches Match { get; set; }
        [DataMember]
        public List<EventsOfMatches> EventsOfMatch { get; set; } = new List<EventsOfMatches>();
        [DataMember]
        public List<PlayersOfMatches> PlayersOfMatch { get; set; } = new List<PlayersOfMatches>();
        [DataMember]
        public int MatchDetailsID { get; set; }
    }
}