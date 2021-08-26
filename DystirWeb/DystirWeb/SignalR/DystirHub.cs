using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DystirWeb
{
    public class DystirHub : Hub
    {
        const string URL = "https://www.dystir.fo";
        //const string URL = "http://localhost:64974";
        public HubConnection DystirHubConnection;

        public DystirHub()
        {
            DystirHubConnection = new HubConnectionBuilder().WithUrl(URL + "/dystirhub").Build();
        }

        public async void SendMatch(Matches match)
        {
            await Clients.All.SendAsync("ReceiveMessage", "match", match.MatchId.ToString() + ";" + match.HomeTeam + ";" + match.AwayTeam + ";" + match.HomeCategoriesName + ";" + match.AwayCategoriesName + ";" + match.StatusId.ToString() + ";" + match.TeamAdminId.ToString());
        }

        public async void SendMatchDetails(MatchDetails matchDetails)
        {
            string matchID = matchDetails?.Match?.MatchId.ToString();
            string element = JsonConvert.SerializeObject(matchDetails);
            await Clients.All.SendAsync("ReceiveMatchDetails", matchID, element);
        }

        public async Task SendUpdateCommand(string matchID)
        {
            await Clients.All.SendAsync("UpdateCommand", matchID);
        }
    }
}