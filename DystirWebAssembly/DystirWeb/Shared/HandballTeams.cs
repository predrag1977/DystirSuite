﻿using System;
using System.Collections.Generic;

namespace DystirWeb.Shared
{
    public partial class HandballTeams
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamLocation { get; set; }
        public string TeamLogo { get; set; }
        public int? TeamId { get; set; }
    }
}
