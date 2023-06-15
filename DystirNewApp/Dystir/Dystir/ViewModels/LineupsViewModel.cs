using System;
using Dystir.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace Dystir.ViewModels
{
	public class LineupsViewModel : DystirViewModel
	{
        ObservableCollection<PlayersInLineups> lineups;
        public ObservableCollection<PlayersInLineups> Lineups
        {
            get { return lineups; }
            set { lineups = value; OnPropertyChanged(); }
        }

        public LineupsViewModel(MatchDetails matchDetails)
		{
            _ = LoadLineupsAsync(matchDetails);
        }

        private async Task LoadLineupsAsync(MatchDetails matchDetails)
        {
            if (Lineups == null)
            {
                var starterPlayers = matchDetails.PlayersOfMatch.Where(x => x.PlayingStatus == 1)
                    .OrderByDescending(x => x.Position == "GK")
                    .ThenBy(x => x.Number);
                var substitutionPlayers = matchDetails.PlayersOfMatch.Where(x => x.PlayingStatus == 2)
                    .OrderByDescending(x => x.Position == "GK")
                    .ThenBy(x => x.Number);

                var homeTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == matchDetails?.Match.HomeTeam));
                var awayTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == matchDetails?.Match.AwayTeam));
                var homeTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == matchDetails?.Match.HomeTeam));
                var awayTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == matchDetails?.Match.AwayTeam));

                var lineups = new ObservableCollection<PlayersInLineups>();

                var biggerLineups = homeTeamLineups.Count >= awayTeamLineups.Count ? homeTeamLineups : awayTeamLineups;
                for (int i = 0; i < biggerLineups.Count; i++)
                {
                    var playerInLineups = new PlayersInLineups()
                    {
                        HomePlayer = homeTeamLineups.Count > i ? homeTeamLineups[i] : new PlayerOfMatch(),
                        AwayPlayer = awayTeamLineups.Count > i ? awayTeamLineups[i] : new PlayerOfMatch()
                    };
                    lineups.Add(playerInLineups);
                }

                var biggerSubstitution = homeTeamSubtitutions.Count >= awayTeamLineups.Count ? homeTeamSubtitutions : awayTeamSubtitutions;
                for (int i = 0; i < biggerSubstitution.Count; i++)
                {
                    var playerInLineups = new PlayersInLineups()
                    {
                        HomePlayer = homeTeamSubtitutions.Count > i ? homeTeamSubtitutions[i] : new PlayerOfMatch(),
                        AwayPlayer = awayTeamSubtitutions.Count > i ? awayTeamSubtitutions[i] : new PlayerOfMatch(),
                        IsFirstSub = i == 0
                    };
                    lineups.Add(playerInLineups);
                }

                Lineups = new ObservableCollection<PlayersInLineups>(lineups);
            }
            await Task.CompletedTask;
        }
    }
}

