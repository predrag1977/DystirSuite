using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Dystir.Models
{
    public class Standing
    {
        public string StandingCompetitionName { get; set; }
        public ObservableCollection<TeamStanding> TeamStandings { get; set; }
    }
}