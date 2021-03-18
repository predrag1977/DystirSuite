using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.Models;

namespace DystirWeb.ModelViews
{
    public class MatchesModelView
    {
        public DateTime SelectedDate { get; internal set; }
        public IEnumerable<IGrouping<string, Matches>> MatchesGroups { get; internal set; }
        public string SelectedCompetition { get; internal set; }
        public IEnumerable<string> CompetitionsList { get; internal set; }
        public string WebSite { get; internal set; }
    }
}