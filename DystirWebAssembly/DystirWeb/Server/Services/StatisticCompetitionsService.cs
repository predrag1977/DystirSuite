
using DystirWeb.Server.DystirDB;
using DystirWeb.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DystirWeb.Services
{
    public class StatisticCompetitionsService
    {
        private readonly DystirService _dystirService;

        public StatisticCompetitionsService(DystirService dystirService)
        {
            _dystirService = dystirService;
        }
        public IEnumerable<CompetitionStatistic> GetCompetitionsStatistic()
        {
            var matchesList = _dystirService.AllMatches;
            var playersList = _dystirService.AllPlayersOfMatches;
            List<CompetitionStatistic> competitionStatisticsList = new List<CompetitionStatistic>();
            var competititionNamesArray = new string[] { "Betri deildin", "1. deild", "Betri deildin kvinnur" };
            foreach (string competititionName in competititionNamesArray)
            {
                competitionStatisticsList.Add(GetStatistics(matchesList, competititionName, playersList));
            }
            return competitionStatisticsList;
        }

        internal CompetitionStatistic GetStatistics(ObservableCollection<Matches> allMatches, string competititionName, ObservableCollection<PlayersOfMatches> playersList)
        {
            CompetitionStatistic competitionStatistic = new CompetitionStatistic();
            try
            {
                competitionStatistic.CompetitionName = competititionName;
                competitionStatistic.TeamStatistics = new List<TeamStatistic>();
                var matches = allMatches?.Where(x => x.MatchTypeName == competititionName && (x.RoundID < 1000));
                //&& (string.Equals(x.HomeTeam, team.TeamName, StringComparison.OrdinalIgnoreCase)
                //|| string.Equals(x.AwayTeam, team.TeamName, StringComparison.OrdinalIgnoreCase)));
                foreach (Matches match in matches)
                {
                    var players = playersList?.Where(p => p.MatchId == match.MatchID);
                    //HOME TEAM STATISTICS
                    bool isNewHomeTeamStatistics = false;
                    TeamStatistic homeTeamFullStatistic = new TeamStatistic();
                    if (competitionStatistic.TeamStatistics.Any(x => x.TeamName == match.HomeTeam))
                    {
                        homeTeamFullStatistic = competitionStatistic.TeamStatistics.FirstOrDefault(x => x.TeamName == match.HomeTeam);
                    }
                    else
                    {
                        isNewHomeTeamStatistics = true;
                        homeTeamFullStatistic.TeamName = match.HomeTeam;
                    }
                    homeTeamFullStatistic.Goal += match.HomeTeamScore ?? 0;
                    homeTeamFullStatistic.Corner += match.HomeTeamCorner ?? 0;
                    homeTeamFullStatistic.OnTarget += match.HomeTeamOnTarget ?? 0 + match.HomeTeamScore ?? 0;
                    competitionStatistic.GoalPlayers.AddRange(players.Where(p => p.TeamName == match.HomeTeam && p.Goal > 0));
                    competitionStatistic.AssistPlayers.AddRange(players.Where(p => p.TeamName == match.HomeTeam && p.Assist > 0));
                    if (isNewHomeTeamStatistics)
                    {
                        competitionStatistic.TeamStatistics.Add(homeTeamFullStatistic);
                    }

                    //AWAY TEAM STATISTICS
                    bool isNewAwayTeamStatistics = false;
                    TeamStatistic awayTeamFullStatistic = new TeamStatistic();
                    if (competitionStatistic.TeamStatistics.Any(x => x.TeamName == match.AwayTeam))
                    {
                        awayTeamFullStatistic = competitionStatistic.TeamStatistics.FirstOrDefault(x => x.TeamName == match.AwayTeam);
                    }
                    else
                    {
                        isNewAwayTeamStatistics = true;
                        awayTeamFullStatistic.TeamName = match.AwayTeam;
                    }
                    awayTeamFullStatistic.Goal += match.AwayTeamScore ?? 0;
                    awayTeamFullStatistic.Corner += match.AwayTeamCorner ?? 0;
                    awayTeamFullStatistic.OnTarget += match.AwayTeamOnTarget ?? 0 + match.AwayTeamScore ?? 0;
                    competitionStatistic.GoalPlayers.AddRange(players.Where(p => p.TeamName == match.AwayTeam && p.Goal > 0));
                    competitionStatistic.AssistPlayers.AddRange(players.Where(p => p.TeamName == match.AwayTeam && p.Assist > 0));
                    if (isNewAwayTeamStatistics)
                    {
                        competitionStatistic.TeamStatistics.Add(awayTeamFullStatistic);
                    }
                }

                var playersGoalGroup = competitionStatistic.GoalPlayers.GroupBy(x => x.FirstName);
                competitionStatistic.GoalPlayers = new List<PlayersOfMatches>();
                foreach (var playerGroup in playersGoalGroup)
                {
                    PlayersOfMatches newPlayers = new PlayersOfMatches()
                    {
                        FirstName = playerGroup.FirstOrDefault().FirstName,
                        TeamName = playerGroup.LastOrDefault().TeamName,
                        Goal = 0
                    };

                    foreach (PlayersOfMatches playerOfMatch in playerGroup)
                    {
                        newPlayers.Goal += playerOfMatch.Goal;
                    }
                    competitionStatistic.GoalPlayers.Add(newPlayers);
                }
                competitionStatistic.GoalPlayers = competitionStatistic.GoalPlayers.OrderByDescending(x => x.Goal).ThenBy(x => x.TeamName).ThenBy(x => x.FirstName).ToList();

                var playersAssistGroup = competitionStatistic.AssistPlayers.GroupBy(x => x.FirstName);
                competitionStatistic.AssistPlayers = new List<PlayersOfMatches>();
                foreach (var playerGroup in playersAssistGroup)
                {
                    PlayersOfMatches newPlayers = new PlayersOfMatches()
                    {
                        FirstName = playerGroup.FirstOrDefault().FirstName,
                        TeamName = playerGroup.LastOrDefault().TeamName,
                        Assist = 0
                    };
                    foreach (PlayersOfMatches playerOfMatch in playerGroup)
                    {
                        newPlayers.Assist += playerOfMatch.Assist;
                    }
                    competitionStatistic.AssistPlayers.Add(newPlayers);
                }
                competitionStatistic.AssistPlayers = competitionStatistic.AssistPlayers.OrderByDescending(x => x.Assist).ThenBy(x => x.TeamName).ThenBy(x => x.FirstName).ToList();
            }
            catch (Exception ex)
            {
                var exception = ex.Message;
            }
            return competitionStatistic;
        }
    }
}
