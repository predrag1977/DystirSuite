using DystirWeb.Shared;
using System.Collections.Generic;
using System.Linq;

namespace DystirWeb.Shared
{
    public class FullMatchDetailsModelView
    {
        public MatchDetails MatchDetails { get; set; }
        public List<SummaryEventOfMatch> Summary { get; set; }
        public List<SummaryEventOfMatch> Commentary { get; set; }
        public Statistic Statistics { get; set; }
        public string WebSite { get; set; }
        public IEnumerable<IGrouping<string, Matches>> MatchesGroups { get; set; }
    }
}