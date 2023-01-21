using System;
using System.Collections.Generic;

namespace DystirWeb.Shared
{
    public partial class MatchTypes
    {
        public int Id { get; set; }
        public string MatchTypeName { get; set; }
        public int? MatchTypeId { get; set; }
        public int? CompetitionID { get; set; }
        public int? OrderID { get; set; }
    }
}
