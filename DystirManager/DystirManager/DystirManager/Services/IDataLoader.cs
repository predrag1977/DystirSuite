using DystirManager.Models;
using DystirManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DystirManager.Services
{
    public interface IDataLoader<T>
    {
        Task<Match> GetMatchAsync(string matchID);
        Task<ObservableCollection<Match>> GetMatchesAsync(string typeOfMatches, MatchesViewModel matchesViewModel);
        Task<ObservableCollection<Standing>> GetStandingsAsync();
        Task<MatchDetails> GetMatchDetailsAsync(string matchID);
        Task<ObservableCollection<Administrator>> GetAdministratorsAsync();
        Task<ObservableCollection<Team>> GetTeamsAsync();
        Task<ObservableCollection<Categorie>> GetCategoriesAsync();
        Task<ObservableCollection<MatchType>> GetMatchTypesAsync();
        Task<ObservableCollection<Squad>> GetSquadsAsync();
        Task<ObservableCollection<Status>> GetStatusesAsync();
        Task<ObservableCollection<Round>> GetRoundsAsync();
        Task<bool> AddMatchAsync(Match match);
        Task<bool> UpdateMatchAsync(Match match);
        Task<bool> DeleteMatchAsync(Match match);
        Task<ObservableCollection<PlayerOfMatch>> GetPlayersOfMatchAsync(Match match);
        Task<bool> AddPlayerOfMatchAsync(PlayerOfMatch playerOfMatch);
        Task<bool> UpdatePlayerOfMatchAsync(PlayerOfMatch playerOfMatch);
        Task<bool> AddEventOfMatchAsync(EventOfMatch eventOfMatch);
        Task<bool> DeleteEventOfMatchAsync(EventOfMatch eventOfMatch);
        Task<bool> UpdateEventOfMatchAsync(EventOfMatch eventOfMatch);
    }
}
