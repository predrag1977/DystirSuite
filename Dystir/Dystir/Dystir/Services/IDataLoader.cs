using Dystir.Models;
using Dystir.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Dystir.Services
{
    public interface IDataLoader<T>
    {
        Task<ObservableCollection<Match>> GetMatchesAsync();
        Task<MatchDetails> GetMatchDetailsAsync(int? matchID);
        Task<ObservableCollection<Standing>> GetStandingsAsync();
        Task<ObservableCollection<CompetitionStatistic>> GetStatisticsAsync();
        Task<ObservableCollection<Sponsor>> GetSponsorsAsync();

        Task<Match> GetMatchAsync(string matchID);
        //Task<ObservableCollection<Match>> GetMatchesAsync(string typeOfMatches, MatchesViewModel matchesViewModel);
        
        
    }
}
