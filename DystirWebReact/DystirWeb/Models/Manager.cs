using System;
namespace DystirWeb.Models
{
	public class Manager
	{
        public int ID { get; set; }
        public int ManagerID { get; set; }
        public string DeviceToken { get; set; }
        public string Name { get; set; }
        public string MatchID { get; set; }
    }
}

