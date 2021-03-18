using DystirWeb.Controllers;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DystirWeb.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private DystirDBContext _dbContext;
        private ObservableCollection<Matches> _matchesList;
        private ObservableCollection<PlayersOfMatches> _playersList;

        public StatisticsController(DystirDBContext dystirDBContext)
        {
            _dbContext = dystirDBContext;
        }

        // GET api/<controller>
        [HttpGet]
        public IEnumerable<CompetitionStatistic> Get()
        {
            DateTime date = new DateTime(DateTime.Now.Year, 1, 1);
            _matchesList = new ObservableCollection<Matches>(_dbContext.Matches?.Where(x => x.MatchTypeId != null
                    && x.MatchActivation != 1
                    && x.MatchActivation != 2
                    && x.Time > date));
            _playersList = new ObservableCollection<PlayersOfMatches>(_dbContext.PlayersOfMatches?
                .Where(x => x.Goal > 0 || x.Assist > 0).ToList().Where(p => _matchesList.Any(m => m.MatchId == p.MatchId)));
            List<CompetitionStatistic> competitionStatisticsList = new List<CompetitionStatistic>();
            var competititionNamesArray = new string[] { "Betri deildin", "1. deild", "Betri deildin kvinnur" };
            foreach (string competititionName in competititionNamesArray)
            {
                competitionStatisticsList.Add(GetStatistics(competititionName));
            }
            return competitionStatisticsList;
        }

        internal CompetitionStatistic GetStatistics(string competititionName)
        {
            CompetitionStatistic competitionStatistic = new CompetitionStatistic();
            try
            {
                competitionStatistic.CompetitionName = competititionName;
                competitionStatistic.TeamStatistics = new List<TeamStatistic>();
                var matches = _matchesList?.Where(x => x.MatchTypeName == competititionName && (x.RoundId < 1000));
                //&& (string.Equals(x.HomeTeam, team.TeamName, StringComparison.OrdinalIgnoreCase)
                //|| string.Equals(x.AwayTeam, team.TeamName, StringComparison.OrdinalIgnoreCase)));
                foreach (Matches match in matches)
                {
                    var players = _playersList?.Where(p => p.MatchId == match.MatchId);
                    //HOME TEAM STATISTICS
                    bool isNewHomeTeamStatistics = false;
                    TeamStatistic homeTeamFullStatistic = new TeamStatistic();
                    if (competitionStatistic.TeamStatistics.Any(x=>x.TeamName == match.HomeTeam)) {
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
                        TeamName = playerGroup.FirstOrDefault().TeamName,
                        Goal = 0
                    };
                    
                    foreach (PlayersOfMatches playerOfMatch in playerGroup)
                    {
                        newPlayers.Goal += playerOfMatch.Goal;
                    }
                    competitionStatistic.GoalPlayers.Add(newPlayers);
                }
                competitionStatistic.GoalPlayers = competitionStatistic.GoalPlayers.OrderByDescending(x => x.Goal).ThenBy(x=>x.TeamName).ThenBy(x=>x.FirstName).ToList();

                var playersAssistGroup = competitionStatistic.AssistPlayers.GroupBy(x => x.FirstName);
                competitionStatistic.AssistPlayers = new List<PlayersOfMatches>();
                foreach (var playerGroup in playersAssistGroup)
                {
                    PlayersOfMatches newPlayers = new PlayersOfMatches()
                    {
                        FirstName = playerGroup.FirstOrDefault().FirstName,
                        TeamName = playerGroup.FirstOrDefault().TeamName,
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

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}