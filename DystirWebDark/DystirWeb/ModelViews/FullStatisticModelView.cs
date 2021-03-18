using DystirWeb.Models;
using System.Collections.Generic;

namespace DystirWeb.ModelViews
{
    public class FullStatisticModelView
    {
        public FullStatisticModelView()
        {
        }

        public string SelectedCompetition { get; set; }
        public IEnumerable<string> CompetitionsList { get; set; }
        public CompetitionStatistic CompetitionStatistic { get; internal set; }
    }
}