using DystirManager.Services;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DystirManager.Models
{
    public class EventOfMatch
    {
        public IDataLoader<Match> GetDataStore()
        {
            return DependencyService.Get<IDataLoader<Match>>() ?? new DataLoader();
        }

        [JsonProperty("EventOfMatchID")]
        public int EventOfMatchID { get; set; }

        [JsonProperty("EventName")]
        public string EventName { get; internal set; }

        [JsonProperty("EventText")]
        public string EventText { get; internal set; }

        [JsonProperty("MainPlayerOfMatchID")]
        public int MainPlayerOfMatchID { get; set; }

        [JsonProperty("SecondPlayerOfMatchID")]
        public int SecondPlayerOfMatchID { get; set; }

        [JsonProperty("MatchID")]
        public int MatchID { get; set; }

        [JsonProperty("EventPeriodID")]
        public int EventPeriodID { get; set; }

        [JsonProperty("EventTeam")]
        public string EventTeam { get; internal set; }

        [JsonProperty("EventMinute")]
        public string EventMinute { get; set; }

        [JsonProperty("EventTotalTime")]
        public string EventTotalTime { get; set; }

        [JsonProperty("EventTime")]
        public DateTime? EventTime { get; set; }

        [JsonProperty("MainPlayerOfMatchNumber")]
        public string MainPlayerOfMatchNumber { get; set; }

        [JsonProperty("SecondPlayerOfMatchNumber")]
        public string SecondPlayerOfMatchNumber { get; set; }

        public int HomeTeamScore { get; internal set; }
        public int AwayTeamScore { get; internal set; }
        public string HomeTeam { get; internal set; }
        public string AwayTeam { get; internal set; }
        public string HomeMainPlayer { get; internal set; }
        public string HomeSecondPlayer { get; internal set; }
        public string AwayMainPlayer { get; internal set; }
        public string AwaySecondPlayer { get; internal set; }
        public bool HomeTeamVisible { get; internal set; }
        public bool AwayTeamVisible { get; internal set; }
        public ObservableCollection<PlayerOfMatch> PlayersList { get; internal set; }
        public ObservableCollection<string> TeamsList { get; internal set; }
        public string MatchTime { get; internal set; }

        internal async Task<bool> AddEventOfMatch()
        {
            try
            {
                await GetDataStore().AddEventOfMatchAsync(this);
                //await GetSelectedLiveMatch(selectedMatch);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        internal async Task<bool> UpdateEventOfMatchAsync()
        {
            try
            {
                await GetDataStore().UpdateEventOfMatchAsync(this);
                //await GetSelectedLiveMatch(selectedMatch);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        internal async Task<bool> DeleteEventOfMatchAsync()
        {
            try
            {
                await GetDataStore().DeleteEventOfMatchAsync(this);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}