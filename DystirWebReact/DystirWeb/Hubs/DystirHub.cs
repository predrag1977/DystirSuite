using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DystirWeb.Hubs
{
    public class DystirHub : Hub<IDystirHub>
    {
        public async Task SendTest(string test)
        {
            await Task.CompletedTask;
        }
    }
}
