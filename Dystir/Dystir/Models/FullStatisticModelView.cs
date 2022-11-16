using System.Collections.Generic;

namespace Dystir.Models
{
    public class FullStatisticModelView
    {
        public string SelectedCompetition { get; set; }
        public IEnumerable<string> CompetitionsList { get; set; }
        public CompetitionStatistic CompetitionStatistic { get; set; }
    }
}