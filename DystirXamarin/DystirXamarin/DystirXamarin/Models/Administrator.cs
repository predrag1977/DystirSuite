using Newtonsoft.Json;

namespace DystirXamarin.Models
{
    public class Administrator
    {
        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("AdministratorID")]
        public int AdministratorID { get; set; }

        [JsonProperty("AdministratorFirstName")]
        public string AdministratorFirstName { get; internal set; }

        [JsonProperty("AdministratorLastName")]
        public string AdministratorLastName { get; internal set; }

        [JsonProperty("AdministratorEmail")]
        public string AdministratorEmail { get; internal set; }

        [JsonProperty("AdministratorPassword")]
        public string AdministratorPassword { get; internal set; }

        [JsonProperty("AdministratorTeamID")]
        public int AdministratorTeamID { get; set; }

        [JsonProperty("AdministratorToken")]
        public string AdministratorToken { get; set; }

    }
}