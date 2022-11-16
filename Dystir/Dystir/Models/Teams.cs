using System;
using System.Collections.Generic;

namespace Dystir.Models
{
    public partial class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamLocation { get; set; }
        public string TeamLogo { get; set; }
        public int? TeamId { get; set; }
    }
}
