using System.Collections.Generic;
using System.Runtime.Serialization;
using Dystir.Models;

namespace Dystir.ViewModels
{

    public class StandingsModelView
    {
        public string SelectedCompetition { get; set; }
        public Standing Standing { get; set; }
        public IEnumerable<string> CompetitionsList { get; set; }
    }

    
}