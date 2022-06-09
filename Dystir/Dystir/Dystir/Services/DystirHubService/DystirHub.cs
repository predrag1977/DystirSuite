using Dystir.Models;
using Dystir.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Dystir.Services.DystirHubService
{
    internal class DystirHub : IDystirHub
    {
        private HubConnection _hubConnection;
        private DystirViewModel dystirViewModel;

        public DystirHub(DystirViewModel dystirViewModel)
        {
            this.dystirViewModel = dystirViewModel;
        }

        public void StartDystirHub()
        {
            _hubConnection = new HubConnectionBuilder().WithUrl("https://www.dystir.fo/DystirHub").Build();
            _hubConnection.Closed += Connection_Closed;
            bool success = TryHubConnectAsync().Result;
        }

        private async Task Connection_Closed(Exception arg)
        {

        }

        private async Task<bool> TryHubConnectAsync()
        {
            bool success = true;
            try
            {
                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
                    _ = _hubConnection.On<string, string>("ReceiveMatchDetails", (matchID, matchDetailsJson) =>
                    {
                        var matchDetails = JsonConvert.DeserializeObject<MatchDetails>(matchDetailsJson);
                        //UpdateMatch(matchID, matchDetails);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                success = false;
            }
            //_viewModel.IsDisconnected = !success;
            return success;
        }
    }
}
