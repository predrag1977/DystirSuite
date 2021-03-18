using System;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace DystirWeb
{
    internal class HubSender
    {
        internal void SendMatch(IHubContext<DystirHub> hubContext, Matches match)
        {
            hubContext.Clients.All.SendAsync("ReceiveMessage", "match", match.MatchId.ToString() + ";" + match.HomeTeam + ";" + match.AwayTeam + ";" + match.HomeCategoriesName + ";" + match.AwayCategoriesName + ";" + match.StatusId.ToString() + ";" + match.TeamAdminId.ToString());
        }

        internal void SendMatchDetails(IHubContext<DystirHub> hubContext, MatchDetails matchDetails)
        {
            string element = JsonConvert.SerializeObject(matchDetails);
            hubContext.Clients.All.SendAsync("ReceiveMatchDetails", matchDetails?.Match?.MatchId.ToString(), element);
        }
    }
}