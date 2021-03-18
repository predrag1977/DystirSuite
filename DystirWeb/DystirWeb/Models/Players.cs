using System;
using System.Collections.Generic;

namespace DystirWeb.Models
{
    public partial class Players
    {
        public int? PlayerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Team { get; set; }
        public DateTime? Birthday { get; set; }
        public string Nationality { get; set; }
        public int? TeamId { get; set; }
    }
}
