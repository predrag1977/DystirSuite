using Newtonsoft.Json;

namespace DystirManager.Models
{
    public class Categorie
    {
        [JsonProperty("CategorieID")]
        public int CategorieID { get; set; }

        [JsonProperty("CategorieName")]
        public string CategorieName { get; internal set; }
    }
}