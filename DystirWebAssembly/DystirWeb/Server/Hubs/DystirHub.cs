using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DystirWeb.Server.Hubs
{
    public class DystirHub : Hub<IDystirHub>
    {
        public async Task SendTest(string test)
        {
            await Task.CompletedTask;
        }
    }
}
