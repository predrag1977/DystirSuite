using System;
using System.Collections.Generic;

namespace DystirWeb.Models
{
    public partial class Administrators
    {
        public int Id { get; set; }
        public string AdministratorFirstName { get; set; }
        public string AdministratorLastName { get; set; }
        public string AdministratorEmail { get; set; }
        public string AdministratorPassword { get; set; }
        public int? AdministratorTeamId { get; set; }
        public int? AdministratorId { get; set; }
    }
}
