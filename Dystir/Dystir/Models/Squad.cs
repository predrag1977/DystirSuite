using System;
using System.Collections.Generic;

namespace Dystir.Models
{
    public partial class Squad
    {
        public int Id { get; set; }
        public string SquadName { get; set; }
        public int? SquadId { get; set; }
        public string SquadShortName { get; set; }
    }
}
