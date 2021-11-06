using System;
using System.Collections.Generic;

namespace DystirWeb.Shared
{
    public partial class Sponsors
    {
        public int Id { get; set; }
        public string SponsorsName { get; set; }
        public string SponsorWebSite { get; set; }
        public int? SponsorId { get; set; }
    }
}
