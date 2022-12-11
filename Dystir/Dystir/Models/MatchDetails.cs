using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Dystir.Models
{
    [DataContract]
    public class MatchDetails
    {
        //*****************************//
        //         PROPERTIES          //
        //*****************************//
        [DataMember]
        public Match Match { get; set; }
        [DataMember]
        public ObservableCollection<EventOfMatch> EventsOfMatch { get; set; }
        [DataMember]
        public ObservableCollection<PlayerOfMatch> PlayersOfMatch { get; set; }
        [DataMember]
        public int MatchDetailsID { get; set; }

        public bool IsDataLoaded = false;
    }
}