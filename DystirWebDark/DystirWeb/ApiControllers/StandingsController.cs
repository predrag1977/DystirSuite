using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DystirWeb.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        private DystirDBContext _dbContext;
        private List<Teams> _teamsList;
        private IQueryable<Matches> _matchesList;

        public StandingsController(DystirDBContext dystirDBContext)
        {
            _dbContext = dystirDBContext;
        }

        // GET api/<controller>
        [HttpGet]
        public IEnumerable<Standing> Get()
        {
            DateTime date = new DateTime(DateTime.Now.Year, 1, 1);
            _teamsList = _dbContext.Teams?.ToList();
            _matchesList = _dbContext.Matches?.Where(x => x.MatchTypeId != null
                    && x.MatchActivation != 1
                    && x.MatchActivation != 2
                    && x.Time > date);
            List<Standing> standingsList = new List<Standing>();
            var competititionNamesArray = new string[] { "Betri deildin", "1. deild", "Betri deildin kvinnur", "2. deild" };
            foreach (string competititionName in competititionNamesArray)
            {
                Standing standing = new Standing()
                {
                    StandingCompetitionName = competititionName,
                    TeamStandings = GetStandings(competititionName)
                };
                foreach (TeamStanding teamStanding in standing.TeamStandings)
                {
                    teamStanding.Position = standing.TeamStandings.ToList().IndexOf(teamStanding) + 1;
                    teamStanding.PositionColor = GetPositionColor(teamStanding);
                }
                standingsList.Add(standing);
            }
            return standingsList;
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

        internal List<TeamStanding> GetStandings(string competitionName)
        {
            List<TeamStanding> teamStandings = new List<TeamStanding>();
            try
            {
                List<Matches> matches = _matchesList?.Where(x => x.MatchTypeName == competitionName && /*(x.StatusId == 13 || x.StatusId == 12)&& */ (x.RoundId < 1000)).ToList();
                foreach (Matches match in matches)
                {
                    if (!teamStandings.Any(x => x.Team.Trim() == match.HomeTeam.Trim()))
                    {
                        TeamStanding teamStanding = new TeamStanding
                        {
                            Team = match.HomeTeam,
                            TeamID = _teamsList?.FirstOrDefault(x => x.TeamName.Trim() == match.HomeTeam.Trim())?.TeamId ?? 0,
                            CompetitionName = competitionName
                        };
                        teamStandings.Add(teamStanding);
                        CalculatePoints(teamStanding, match, match.HomeTeamScore, match.AwayTeamScore);
                    }
                    else
                    {
                        TeamStanding teamStanding = teamStandings.FirstOrDefault(x => x.Team.Trim() == match.HomeTeam.Trim());
                        CalculatePoints(teamStanding, match, match.HomeTeamScore, match.AwayTeamScore);
                    }

                    if (!teamStandings.Any(x => x.Team.Trim() == match.AwayTeam.Trim()))
                    {
                        TeamStanding teamStanding = new TeamStanding
                        {
                            Team = match.AwayTeam,
                            TeamID = _teamsList?.FirstOrDefault(x=>x.TeamName.Trim() == match.AwayTeam.Trim())?.TeamId ?? 0,
                            CompetitionName = competitionName
                        };
                        teamStandings.Add(teamStanding);
                        CalculatePoints(teamStanding, match, match.AwayTeamScore, match.HomeTeamScore);
                    }
                    else
                    {
                        TeamStanding teamStanding = teamStandings.FirstOrDefault(x => x.Team.Trim() == match.AwayTeam.Trim());
                        CalculatePoints(teamStanding, match, match.AwayTeamScore, match.HomeTeamScore);
                    }
                }
            }
            catch (Exception ex)
            {
                var exception = ex.Message;

            }

            //ReducePenaltiesPoint(teamStandings);

            teamStandings.RemoveAll(x => x.TeamID == 0);
            return teamStandings.OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalScored)
                .ThenBy(x => x.GoalAgainst).ToList();
        }

        private void CalculatePoints(TeamStanding teamStanding, Matches match, int? mainTeamScore, int? opponentTeamScore)
        {
            //teamStanding.IsLive = !teamStanding.IsLive && match.StatusId > 1 && match.StatusId < 5;

            //if (match.StatusId < 2 || match.StatusId > 13)
            //{
            //    return;
            //}
            if (match.StatusId != 12 && match.StatusId != 13)
            {
                return;
            }
            teamStanding.MatchesNo += 1;
            if (mainTeamScore != null && opponentTeamScore != null)
            {
                if (mainTeamScore > opponentTeamScore)
                {
                    teamStanding.Points += 3;
                    teamStanding.Victories += 1;
                }
                else if (mainTeamScore == opponentTeamScore)
                {
                    teamStanding.Points += 1;
                    teamStanding.Draws += 1;
                }
                else if (mainTeamScore < opponentTeamScore)
                {
                    teamStanding.Losses += 1;
                }
                teamStanding.GoalScored += mainTeamScore;
                teamStanding.GoalAgainst += opponentTeamScore;
                teamStanding.GoalDifference = teamStanding.GoalScored - teamStanding.GoalAgainst;
            }
            var result = (double)teamStanding.Points / (teamStanding.MatchesNo * 3) * 100;
            teamStanding.PointsProcent = (int)Math.Round((double)result, 2);
        }

        private void ReducePenaltiesPoint(List<TeamStanding> teamStandingsList)
        {
            List<Tuple<string, string, int>> teamPenatiesPointList = new List<Tuple<string, string, int>>
            {
                new Tuple<string, string, int>("2. deild", "B68", 3)
            };
            foreach (TeamStanding teamStanding in teamStandingsList)
            {
                var teamFound = teamPenatiesPointList.Find(x => x.Item1 == teamStanding.CompetitionName && x.Item2 == teamStanding.Team);
                if (teamFound != null)
                {
                    teamStanding.Points -= teamFound.Item3;
                }
            }
        }

        private string GetPositionColor(TeamStanding teamStanding)
        {
            if (teamStanding.CompetitionName == "Betri deildin")
            {
                switch (teamStanding.Position)
                {
                    case 1:
                        return "gold";
                    case 3:
                        return "green";
                    case 8:
                        return "darkred";
                    default:
                        return "dimgray";
                }
            }
            else if (teamStanding.CompetitionName == "Betri deildin kvinnur")
            {
                switch (teamStanding.Position)
                {
                    case 1:
                        return "gold";
                    default:
                        return "dimgray";
                }
            }
            else if (teamStanding.CompetitionName == "VFF Cup")
            {
                return "dimgray";
            }
            else
            {
                switch (teamStanding.Position)
                {
                    case 2:
                        return "green";
                    case 8:
                        return "darkred";
                    default:
                        return "dimgray";
                }
            }
        }
    }
}