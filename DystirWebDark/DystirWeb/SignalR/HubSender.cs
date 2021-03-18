using System;
using System.Collections.Generic;
using System.Linq;
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

        internal void SendFullMatchesData(IHubContext<DystirHub> hubContext, MatchDetails matchDetails, List<Matches> matchesList )
        {
            string matchDetailsJson = JsonConvert.SerializeObject(matchDetails);
            string matchesListJson = JsonConvert.SerializeObject(matchesList);
            hubContext.Clients.All.SendAsync("ReceiveFullMatchesData", matchDetails?.Match?.MatchId.ToString(), matchDetailsJson, matchesListJson);
        }
    }
}