using System;
using System.Collections.Generic;

namespace DystirWeb.Models
{
    public partial class Categories
    {
        public int Id { get; set; }
        public string CategorieName { get; set; }
        public int? CategorieId { get; set; }
    }
}
