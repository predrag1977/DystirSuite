using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dystir.Models;
using Xamarin.Forms;

namespace Dystir.Services
{
    public class LiveStandingService
    {
        //**************************//
        //        PROPERTIES        //
        //**************************//
        private readonly DystirService _dystirService;

        //**********************//
        //     CONSTRUCTOR      //
        //**********************//
        public LiveStandingService()
        {
            _dystirService = DependencyService.Get<DystirService>();
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public Standing GetStanding(string competititionName)
        {
            var matchesList = _dystirService.AllMatches.Select(x => x.Match).ToList<Match>();
            Standing standing = new Standing()
            {
                StandingCompetitionName = competititionName,
                TeamStandings = GetStandings(competititionName, matchesList)
            };
            foreach (TeamStanding teamStanding in standing.TeamStandings)
            {
                teamStanding.Position = standing.TeamStandings.ToList().IndexOf(teamStanding) + 1;
                teamStanding.PositionColor = GetPositionColor(teamStanding);
            }
            return standing;
        }

        public static ObservableCollection<TeamStanding> GetStandings(string competitionName, IEnumerable<Match> matchesList)
        {
            List<TeamStanding> teamStandings = new List<TeamStanding>();
            try
            {
                List<Match> matches = matchesList?.Where(x => x.MatchTypeName == competitionName && /*(x.StatusId == 13 || x.StatusId == 12)&& */ (x.RoundID < 1000)).ToList();
                foreach (Match match in matches)
                {
                    if (!teamStandings.Any(x => x.Team.Trim() == match.HomeTeam.Trim()))
                    {
                        TeamStanding teamStanding = new TeamStanding
                        {
                            Team = match.HomeTeam,
                            TeamID = match.HomeTeamID ?? 0,
                            CompetitionName = competitionName,
                            IsLive = match.StatusID > 1 && match.StatusID < 6
                        };
                        teamStandings.Add(teamStanding);
                        CalculatePoints(teamStanding, match, match.HomeTeamScore, match.AwayTeamScore);
                    }
                    else
                    {
                        TeamStanding teamStanding = teamStandings.FirstOrDefault(x => x.Team.Trim() == match.HomeTeam.Trim());
                        teamStanding.IsLive = match.StatusID > 1 && match.StatusID < 6;
                        CalculatePoints(teamStanding, match, match.HomeTeamScore, match.AwayTeamScore);
                    }

                    if (!teamStandings.Any(x => x.Team.Trim() == match.AwayTeam.Trim()))
                    {
                        TeamStanding teamStanding = new TeamStanding
                        {
                            Team = match.AwayTeam,
                            TeamID = match.AwayTeamID ?? 0,
                            CompetitionName = competitionName,
                            IsLive = match.StatusID > 1 && match.StatusID < 6
                        };
                        teamStandings.Add(teamStanding);
                        LiveStandingService.CalculatePoints(teamStanding, match, match.AwayTeamScore, match.HomeTeamScore);
                    }
                    else
                    {
                        TeamStanding teamStanding = teamStandings.FirstOrDefault(x => x.Team.Trim() == match.AwayTeam.Trim());
                        teamStanding.IsLive = match.StatusID > 1 && match.StatusID < 6;
                        LiveStandingService.CalculatePoints(teamStanding, match, match.AwayTeamScore, match.HomeTeamScore);
                    }
                }
            }
            catch (Exception ex)
            {
                var exception = ex.Message;
            }

            //ReducePenaltiesPoint(teamStandings);

            teamStandings.RemoveAll(x => x.TeamID == 0);
            return new ObservableCollection<TeamStanding>(teamStandings.OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalScored)
                .ThenBy(x => x.GoalAgainst).ThenBy(x => x.Team));
        }

        //**************************//
        //      PRIVATE METHODS     //
        //**************************//
        private static void CalculatePoints(TeamStanding teamStanding, Match match, int? mainTeamScore, int? opponentTeamScore)
        {
            if (match.StatusID < 2 || match.StatusID > 13)
            {
                return;
            }
            teamStanding.MatchesNo += 1;
            teamStanding.MatchesNo = teamStanding.IsLive ? teamStanding.MatchesNo - 1 : teamStanding.MatchesNo;
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

        private static void ReducePenaltiesPoint(List<TeamStanding> teamStandingsList)
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
            else if (teamStanding.CompetitionName == "VFF Cup kvinnur")
            {
                return "dimgray";
            }
            else if (teamStanding.CompetitionName == "2. deild")
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
