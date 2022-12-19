using System.Collections.ObjectModel;
using System;

namespace Dystir.Models
{
    public class Lineups : List<PlayersInRow>
    {
        public Lineups(ObservableCollection<PlayerOfMatch> homeTeamLineups, ObservableCollection<PlayerOfMatch> awayTeamLineups, ObservableCollection<PlayerOfMatch> homeTeamSubtitutions, ObservableCollection<PlayerOfMatch> awayTeamSubtitutions)
        {
            var biggerLineups = homeTeamLineups.Count >= awayTeamLineups.Count ? homeTeamLineups : awayTeamLineups;
            int rowIndex = 0;
            for (int i = 0; i < biggerLineups.Count; i++)
            {
                var playerInRow = new PlayersInRow()
                {
                    HomePlayer = homeTeamLineups.Count > i ? homeTeamLineups[i] : new PlayerOfMatch(),
                    AwayPlayer = awayTeamLineups.Count > i ? awayTeamLineups[i] : new PlayerOfMatch(),
                    RowIndex = rowIndex++
                };
                Add(playerInRow);
            }

            var biggerSubstitution = homeTeamSubtitutions.Count >= awayTeamLineups.Count ? homeTeamSubtitutions : awayTeamSubtitutions;
            for (int i = 0; i < biggerSubstitution.Count; i++)
            {
                var playerInRow = new PlayersInRow()
                {
                    HomePlayer = homeTeamSubtitutions.Count > i ? homeTeamSubtitutions[i] : new PlayerOfMatch(),
                    AwayPlayer = awayTeamSubtitutions.Count > i ? awayTeamSubtitutions[i] : new PlayerOfMatch(),
                    RowIndex = rowIndex++,
                    IsFirstSub = i == 0
                };
                Add(playerInRow);
            }
        }
    }
}

