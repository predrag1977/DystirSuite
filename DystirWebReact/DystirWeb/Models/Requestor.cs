using System;
using System.Collections.Generic;

namespace DystirWeb.Shared
{
    public partial class Requestor
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
        public int? Active { get; set; }
    }
}
