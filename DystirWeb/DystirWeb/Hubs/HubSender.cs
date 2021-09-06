using DystirWeb.Models;
using DystirWeb.ModelViews;
using DystirWeb.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace DystirWeb
{
    internal class HubSender
    {
        internal void SendMatch(IHubContext<DystirHub> hubContext, Matches match)
        {
            hubContext.Clients.All.SendAsync("ReceiveMessage", "match", match.MatchID.ToString() + ";" + match.HomeTeam + ";" + match.AwayTeam + ";" + match.HomeCategoriesName + ";" + match.AwayCategoriesName + ";" + match.StatusID.ToString() + ";" + match.TeamAdminId.ToString());
        }

        internal void SendMatchDetails(IHubContext<DystirHub> hubContext, MatchDetails matchDetails)
        {
            string element = JsonConvert.SerializeObject(matchDetails);
            hubContext.Clients.All.SendAsync("ReceiveMatchDetails", matchDetails?.Match?.MatchID.ToString(), element);
        }

        internal void SendUpdateCommand(IHubContext<DystirHub> hubContext, string matchID)
        {
            hubContext.Clients.All.SendAsync("UpdateCommand", matchID);
        }
    }
}