using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DystirWeb
{
    public class DystirHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMatchDetails(string action, string eventJson)
        {
            await Clients.All.SendAsync("ReceiveMatchDetails", action, eventJson);
        }
    }
}