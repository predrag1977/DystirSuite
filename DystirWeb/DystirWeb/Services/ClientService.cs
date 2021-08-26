using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace DystirWeb.Services
{
    public class ClientService
    {
        public bool IsConnected = true;
        public event Action OnConnected;
        public event Action OnReconnected;
        public event Action OnDisconnected;
        public event Action<string> OnUpdated;
        public void NotifyOnConnected() => OnConnected?.Invoke();
        public void NotifyOnReconnected() => OnReconnected?.Invoke();
        public void NotifyOnDisconnected() => OnDisconnected?.Invoke();
        public void NotifyOnUpdated(string matchID) => OnUpdated?.Invoke(matchID);

        [JSInvokable]
        public Task<string> ConnectionConnected()
        {
            IsConnected = true;
            NotifyOnConnected();
            return Task.FromResult("Connected");
        }

        [JSInvokable]
        public Task<string> ConnectionReconnected()
        {
            IsConnected = true;
            NotifyOnReconnected();
            return Task.FromResult("Reconnected");
        }

        [JSInvokable]
        public Task<string> ConnectionDisconnected()
        {
            IsConnected = false;
            NotifyOnDisconnected();
            return Task.FromResult("Disconnected");
        }

        [JSInvokable]
        public Task<string> ConnectionUpdated(string matchID)
        {
            NotifyOnUpdated(matchID);
            return Task.FromResult("Updated");
        }
    }
}
