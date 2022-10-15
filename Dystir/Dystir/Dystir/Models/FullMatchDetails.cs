using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dystir.Models
{
    public class FullMatchDetails
    {
        public MatchDetails MatchDetails { get; set; }
        public ObservableCollection<SummaryEventOfMatch> Summary { get; set; }
        public ObservableCollection<SummaryEventOfMatch> Commentary { get; set; }
        public Statistic Statistics { get; set; }
        public string WebSite { get; set; }
    }
}