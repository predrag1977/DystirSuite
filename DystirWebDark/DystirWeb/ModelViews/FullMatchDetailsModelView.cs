using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.Controllers;
using DystirWeb.Models;

namespace DystirWeb.ModelViews
{
    public class FullMatchDetailsModelView
    {
        public MatchDetails MatchDetails { get; internal set; }
        public List<Matches> MatchesListSelection { get; internal set; }
        public List<SummaryEventOfMatch> Summary { get; internal set; }
        public List<SummaryEventOfMatch> Commentary { get; internal set; }
        public Statistic Statistics { get; internal set; }
        public string WebSite { get; internal set; }
        public IEnumerable<IGrouping<string, Matches>> MatchesGroups { get; internal set; }
    }
}