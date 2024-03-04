using DystirWeb.Models;
using DystirWeb.Shared;

namespace DystirWeb.Services
{
    public class StandingService
    {
        private readonly DystirService _dystirService;

        public StandingService (DystirService dystirService)
        {
            _dystirService = dystirService;
        }

        internal IEnumerable<Standing> GetStandings()
        {
            var fromDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
            var matchesList = _dystirService.AllMatches.Where(x => x.Time > fromDate
                && x.MatchActivation != 1
                && x.MatchActivation != 2);
            var teamsList = _dystirService.AllTeams;

            List<Standing> standingsList = new List<Standing>();
            var competititionsArray = _dystirService.AllCompetitions?
                .Where(x=>x.CompetitionID > 0)
                .OrderBy(x=>x.OrderID).ToList() ?? new List<MatchTypes>();
            foreach (MatchTypes competitition in competititionsArray)
            {
                Standing standing = new Standing()
                {
                    StandingCompetitionId = competititionsArray.IndexOf(competitition).ToString(),
                    StandingTypeID = competitition.CompetitionID ?? 0,
                    StandingCompetitionName = competitition.MatchTypeName,
                    TeamStandings = GetStandings(competitition.MatchTypeName, teamsList, matchesList)
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

        internal List<TeamStanding> GetStandings(string competitionName, IEnumerable<Teams> teamsList, IEnumerable<Matches> matchesList)
        {
            List<TeamStanding> teamStandings = new List<TeamStanding>();
            try
            {
                List<Matches> matches = matchesList?.Where(x => x.MatchTypeName == competitionName && /*(x.StatusId == 13 || x.StatusId == 12)&& */ (x.RoundID < 1000)).ToList();
                
                foreach (Matches match in matches)
                {
                    if (!teamStandings.Any(x => x.Team.Trim() == match.HomeTeam.Trim()))
                    {
                        TeamStanding teamStanding = new TeamStanding
                        {
                            Team = match.HomeTeam,
                            TeamID = teamsList?.FirstOrDefault(x => x.TeamName.Trim() == match.HomeTeam.Trim())?.TeamId ?? 0,
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
                            TeamID = teamsList?.FirstOrDefault(x => x.TeamName.Trim() == match.AwayTeam.Trim())?.TeamId ?? 0,
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
                .ThenBy(x => x.GoalAgainst).ThenBy(x=> x.Team).ToList();
        }

        private void CalculatePoints(TeamStanding teamStanding, Matches match, int? mainTeamScore, int? opponentTeamScore)
        {
            if(teamStanding.IsLive == false)
            {
                teamStanding.IsLive = (match.StatusID > 1 && match.StatusID < 6);
            }
            if (match.StatusID < 2 || match.StatusID > 13)
            {
                return;
            }
            //if (match.StatusID != 12 && match.StatusID != 13)
            //{
            //    return;
            //}
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

        private void ReducePenaltiesPointTest(List<TeamStanding> teamStandingsList)
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

        private static string GetPositionColor(TeamStanding teamStanding)
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
            else if (teamStanding.CompetitionName == "VFF Cup kvinnur")
            {
                return "dimgray";
            }
            else if(teamStanding.CompetitionName == "2. deild")
            {
                switch (teamStanding.Position)
                {
                    case 2:
                        return "green";
                    case 10:
                        return "darkred";
                    default:
                        return "dimgray";
                }
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
