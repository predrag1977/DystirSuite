using DystirXamarin.Models;
using DystirXamarin.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DystirXamarin.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddMatchAsync(T item);
        Task<bool> UpdateMatchAsync(T item);
        Task<bool> DeleteMatchAsync(T item);
        Task<Match> GetMatchAsync(Match match);
        Task<ObservableCollection<Match>> GetMatchesAsync(string typeOfMatches, MatchesViewModel matchesViewModel);
        Task<ObservableCollection<Team>> GetTeamsAsync();
        Task<ObservableCollection<Categorie>> GetCategoriesAsync();
        Task<ObservableCollection<MatchType>> GetMatchTypesAsync();
        Task<ObservableCollection<Squad>> GetSquadsAsync();
        Task<ObservableCollection<Status>> GetStatusesAsync();
        Task<ObservableCollection<Round>> GetRoundsAsync();
        Task<List<Player>> GetPlayersAsync(Match match);
        Task<ObservableCollection<PlayerOfMatch>> GetPlayersOfMatchAsync(Match match);
        Task<bool> AddPlayerOfMatchAsync(PlayerOfMatch playerOfMatch);
        Task<bool> UpdatePlayerOfMatchAsync(PlayerOfMatch player);
        Task<bool> PullLatestAsync();
        Task<bool> SyncAsync();
        Task<ObservableCollection<EventOfMatch>> GetEventsOfMatchAsync(Match match);
        Task<bool> AddEventOfMatchAsync(EventOfMatch eventOfMatch);
        Task<bool> DeleteEventOfMatchAsync(EventOfMatch eventOfMatch);
        Task<ObservableCollection<Administrator>> GetAdministratorsAsync();
        Task<bool> UpdateEventOfMatchAsync(EventOfMatch eventOfMatch);
        Task<MatchDetails> GetMatchDetailsAsync(string matchID);
    }
}
