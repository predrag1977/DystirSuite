using System.Collections.ObjectModel;

namespace Dystir.Models
{
    public class Standing
    {
        public string StandingCompetitionName { get; set; }
        public ObservableCollection<TeamStanding> TeamStandings { get; set; }
        public bool IsHeaderVisible { get; set; }
    }
}