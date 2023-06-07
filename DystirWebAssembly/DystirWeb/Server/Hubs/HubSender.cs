using System;
using DystirWeb.Server.Hubs;
using DystirWeb.Shared;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace DystirWeb
{
    internal class HubSender
    {
        internal static void SendMatch(IHubContext<DystirHub> hubContext, Matches match)
        {
            hubContext.Clients.All.SendAsync("ReceiveMessage", "match", match.MatchID.ToString() + ";" + match.HomeTeam + ";" + match.AwayTeam + ";" + match.HomeCategoriesName + ";" + match.AwayCategoriesName + ";" + match.StatusID.ToString() + ";" + match.TeamAdminID.ToString());
        }

        internal static void SendMatchDetails(IHubContext<DystirHub> hubContext, MatchDetails matchDetails)
        {
            string element = JsonConvert.SerializeObject(matchDetails);
            hubContext.Clients.All.SendAsync("ReceiveMatchDetails", matchDetails?.MatchDetailsID.ToString(), element);
        }

        internal static void SendRefreshData(IHubContext<DystirHub> hubContext)
        {
            hubContext.Clients.All.SendAsync("RefreshData");
        }
    }
}