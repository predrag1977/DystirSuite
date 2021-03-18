using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.ApiControllers;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DystirWeb.Controllers
{
    public class InfoMatchDetailsController : Controller
    {
        private DystirDBContext dbContext;

        public InfoMatchDetailsController(DystirDBContext dystirDBContext)
        {
            dbContext = dystirDBContext;
        }

        [Route("InfoDystarGreiningar/{matchID}")]
        public IActionResult Index(string matchID)
        {
            return View("~/Views/InfoMatchDetails/Index.cshtml", GetMatchDetails(matchID));
        }

        public IActionResult LoadMatchDetails(string matchID)
        {
            try
            {
                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        public IActionResult SelectedMatchDetails(string selectedMatchID)
        {
            FullMatchDetailsModelView fullMatchDetailsModelView = GetMatchDetails(selectedMatchID);
            return PartialView("~/Views/PartialViews/InfoMatchDetailsPartialView.cshtml", fullMatchDetailsModelView);
        }

        public IActionResult GetSelectedMatchDetailsView(string matchDetails, string matchesList)
        {
            FullMatchDetailsModelView fullMatchDetailsModelView = GetMatchDetailsFromJson(matchDetails, matchesList);
            return PartialView("~/Views/PartialViews/InfoMatchDetailsPartialView.cshtml", fullMatchDetailsModelView);
        }

        private FullMatchDetailsModelView GetMatchDetailsFromJson(string matchDetailsJson, string matchesListJson)
        {
            FullMatchDetailsModelView fullMatchDetailsModelView = new FullMatchDetailsModelView();
            try
            {
                List<Matches> allMatches = JsonConvert.DeserializeObject<List<Matches>>(matchesListJson);
                MatchDetails matchDetails = JsonConvert.DeserializeObject<MatchDetails>(matchDetailsJson);
                Matches match = matchDetails.Match;
                DateTime date = match.Time.Value.Date;
                fullMatchDetailsModelView.MatchesListSelection = GetMatchesFromJson(allMatches);
                fullMatchDetailsModelView.MatchDetails = matchDetails;
                fullMatchDetailsModelView.Summary = GetSummary(fullMatchDetailsModelView.MatchDetails);
                fullMatchDetailsModelView.Commentary = GetCommentary(fullMatchDetailsModelView.MatchDetails);
                fullMatchDetailsModelView.Statistics = GetStatistics(fullMatchDetailsModelView.MatchDetails.EventsOfMatch, fullMatchDetailsModelView.MatchDetails.Match);
                fullMatchDetailsModelView.MatchesGroups = fullMatchDetailsModelView.MatchesListSelection?.GroupBy(x => x.MatchTypeName);
            }
            catch (Exception)
            {

            }
            return fullMatchDetailsModelView;
        }

        public IActionResult MatchListSelectionDetails()
        {
            FullMatchDetailsModelView fullMatchDetailsModelView = new FullMatchDetailsModelView();
            fullMatchDetailsModelView.MatchesListSelection = GetMatches();
            fullMatchDetailsModelView.MatchesGroups = fullMatchDetailsModelView.MatchesListSelection?.GroupBy(x => x.MatchTypeName);
            return PartialView("~/Views/PartialViews/InfoMatchDetailsSelectionListView.cshtml", fullMatchDetailsModelView);
        }

        public IActionResult GetMatchListSelectionDetailsView(string matchesList)
        {
            FullMatchDetailsModelView fullMatchDetailsModelView = new FullMatchDetailsModelView();
            List<Matches> allMatches = JsonConvert.DeserializeObject<List<Matches>>(matchesList);
            fullMatchDetailsModelView.MatchesListSelection = GetMatchesFromJson(allMatches);
            fullMatchDetailsModelView.MatchesGroups = fullMatchDetailsModelView.MatchesListSelection?.GroupBy(x => x.MatchTypeName);
            return PartialView("~/Views/PartialViews/InfoMatchDetailsSelectionListView.cshtml", fullMatchDetailsModelView);
        }

        private FullMatchDetailsModelView GetMatchDetails(string matchID)
        {
            FullMatchDetailsModelView fullMatchDetailsModelView = new FullMatchDetailsModelView();
            try
            {
                Matches match = (new MatchesController(null, dbContext).GetMatches(int.Parse(matchID)) as OkObjectResult).Value as Matches;
                DateTime date = match.Time.Value.Date;
                fullMatchDetailsModelView.MatchesListSelection = GetMatches();
                fullMatchDetailsModelView.MatchDetails = new MatchDetailsController(dbContext).Get(int.Parse(matchID));
                fullMatchDetailsModelView.Summary = GetSummary(fullMatchDetailsModelView.MatchDetails);
                fullMatchDetailsModelView.Commentary = GetCommentary(fullMatchDetailsModelView.MatchDetails);
                fullMatchDetailsModelView.Statistics = GetStatistics(fullMatchDetailsModelView.MatchDetails.EventsOfMatch, fullMatchDetailsModelView.MatchDetails.Match);
                fullMatchDetailsModelView.MatchesGroups = fullMatchDetailsModelView.MatchesListSelection?.GroupBy(x => x.MatchTypeName);
            }
            catch (Exception)
            {

            }
            return fullMatchDetailsModelView;
        }

        private List<Matches> GetMatches()
        {
            List<Matches> matchesList = new List<Matches>();
            try
            {
                var fromDate = DateTime.UtcNow.Date.AddDays(-30);
                var toDate = DateTime.UtcNow.Date.AddDays(60);
                matchesList = dbContext.Matches?.Where(x => x.MatchTypeId == 1 || x.MatchTypeId == 101 || x.MatchTypeId == 2 
                || x.MatchTypeId == 102 || x.MatchTypeId == 5 || x.MatchTypeId == 4 || x.MatchTypeId == 12 || x.MatchTypeId == 13 
                || x.MatchTypeId == 301 || x.MatchTypeId == 302 || x.MatchTypeId == 303 || x.MatchTypeId == 16 || x.MatchTypeId == 0 || x.MatchTypeId == 40)
                    .OrderBy(x => x.MatchTypeId)
                    .ThenBy(x => x.Time)
                    //.ThenBy(x => x.StatusId)
                    .Where(x => x.Time.Value.Date.Date >= fromDate && x.Time.Value.Date.Date <= toDate 
                    //.Where(x => x.Time.Value.Date.Date == DateTime.UtcNow.Date
                    && x.MatchActivation != 1 && x.MatchActivation != 2).ToList();
            }
            catch (Exception)
            {

            }
            return matchesList;
        }

        private List<Matches> GetMatchesFromJson(List<Matches> allMatches)
        {
            List<Matches> matchesList = new List<Matches>();
            try
            {
                var fromDate = DateTime.UtcNow.Date.AddDays(-30);
                var toDate = DateTime.UtcNow.Date.AddDays(60);
                matchesList = allMatches?.Where(x => x.MatchTypeId == 1 || x.MatchTypeId == 101 || x.MatchTypeId == 2
                || x.MatchTypeId == 102 || x.MatchTypeId == 5 || x.MatchTypeId == 4 || x.MatchTypeId == 12 || x.MatchTypeId == 13
                || x.MatchTypeId == 301 || x.MatchTypeId == 302 || x.MatchTypeId == 303 || x.MatchTypeId == 16 || x.MatchTypeId == 0 || x.MatchTypeId == 40)
                    .OrderBy(x => x.MatchTypeId)
                    .ThenBy(x => x.Time)
                    //.ThenBy(x => x.StatusId)
                    .Where(x => x.Time.Value.Date.Date >= fromDate && x.Time.Value.Date.Date <= toDate 
                    //.Where(x => x.Time.Value.Date.Date == DateTime.UtcNow.Date
                    && x.MatchActivation != 1 && x.MatchActivation != 2).ToList();
            }
            catch (Exception)
            {

            }
            return matchesList;
        }

        private List<SummaryEventOfMatch> GetSummary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> listSummaryEvents = GetSummaryEventsList(matchDetails);
            if (matchDetails.Match?.StatusId < 12)
            {
                listSummaryEvents.Reverse();
            }
            return listSummaryEvents;
        }

        internal List<SummaryEventOfMatch> GetSummaryEventsList(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> summaryEventOfMatchesList = new List<SummaryEventOfMatch>();
            Matches selectedMatch = matchDetails.Match;
            var homeTeamPlayers = matchDetails.PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.HomeTeam.Trim());
            var awayTeamPlayers = matchDetails.PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.AwayTeam.Trim());
            var eventsList = matchDetails.EventsOfMatch?.Where(x =>
                 x.EventName == "GOAL"
                 || x.EventName == "OWNGOAL"
                 || x.EventName == "PENALTYSCORED"
                 || x.EventName == "PENALTYMISSED"
                 || x.EventName == "YELLOW"
                 || x.EventName == "RED"
                 || x.EventName == "BIGCHANCE"
                 //|| x.EventName == "SUBSTITUTION"
                 || x.EventName == "ASSIST").ToList();
            int homeScore = 0;
            int awayScore = 0;
            foreach (var eventOfMatch in eventsList ?? new List<EventsOfMatches>())
            {
                SummaryEventOfMatch summaryEventOfMatch = new SummaryEventOfMatch()
                {
                    EventOfMatch = eventOfMatch,
                    HomeTeamScore = 0,
                    AwayTeamScore = 0,
                    HomeTeam = eventOfMatch?.EventTeam == selectedMatch.HomeTeam ? selectedMatch.HomeTeam : string.Empty,
                    AwayTeam = eventOfMatch?.EventTeam == selectedMatch.AwayTeam ? selectedMatch.AwayTeam : string.Empty,
                    EventName = eventOfMatch?.EventName,
                    EventMinute = eventOfMatch?.EventMinute,

                };
                PlayersOfMatches mainPlayerOfMatch = matchDetails.PlayersOfMatch.Find(x => x.PlayerOfMatchId == eventOfMatch.MainPlayerOfMatchId);
                string mainPlayerFullName = (mainPlayerOfMatch?.FirstName?.Trim() + " " + mainPlayerOfMatch?.Lastname?.Trim())?.Trim();
                PlayersOfMatches secondPlayerOfMatch = matchDetails.PlayersOfMatch.Find(x => x.PlayerOfMatchId == eventOfMatch.SecondPlayerOfMatchId);
                string secondPlayerFullName = (secondPlayerOfMatch?.FirstName?.Trim() + " " + secondPlayerOfMatch?.Lastname?.Trim())?.Trim();
                if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                {
                    summaryEventOfMatch.HomeMainPlayer = mainPlayerFullName;
                    summaryEventOfMatch.HomeSecondPlayer = secondPlayerFullName;
                }
                else
                {
                    summaryEventOfMatch.AwayMainPlayer = mainPlayerFullName;
                    summaryEventOfMatch.AwaySecondPlayer = secondPlayerFullName;
                }
                summaryEventOfMatch.HomeTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.HomeTeam);
                summaryEventOfMatch.AwayTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.AwayTeam);
                if (IsGoal(eventOfMatch))
                {
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                    {
                        homeScore += 1;
                    }
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())
                    {
                        awayScore += 1;
                    }
                }
                summaryEventOfMatch.HomeTeamScore = homeScore;
                summaryEventOfMatch.AwayTeamScore = awayScore;

                if (eventOfMatch.EventName == "GOAL")
                {
                    int eventIndex = eventsList.IndexOf(eventOfMatch);
                    if (eventIndex + 1 < eventsList.Count)
                    {
                        EventsOfMatches nextEvent = eventsList[eventIndex + 1];
                        if (nextEvent.EventName == "ASSIST")
                        {
                            PlayersOfMatches assistPlayerOfMatch = matchDetails.PlayersOfMatch.Find(x => x.PlayerOfMatchId == nextEvent.MainPlayerOfMatchId);
                            string assistPlayerFullName = (assistPlayerOfMatch?.FirstName?.Trim() + " " + assistPlayerOfMatch?.Lastname?.Trim())?.Trim();
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                            {
                                summaryEventOfMatch.HomeSecondPlayer = assistPlayerFullName;
                            }
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())
                            {
                                summaryEventOfMatch.AwaySecondPlayer = assistPlayerFullName;
                            }
                        }
                    }
                }
                summaryEventOfMatchesList.Add(summaryEventOfMatch);
            }

            return summaryEventOfMatchesList.ToList();
        }

        private bool IsGoal(EventsOfMatches matchEvent)
        {
            return matchEvent.EventName == "GOAL"
                || matchEvent.EventName == "OWNGOAL"
                || matchEvent.EventName == "PENALTYSCORED";
        }

        private List<SummaryEventOfMatch> GetCommentary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> summaryEventOfMatchesList = new List<SummaryEventOfMatch>();
            var eventsList = matchDetails.EventsOfMatch?.Where(x => x != null).ToList();
            int homeScore = 0;
            int awayScore = 0;
            foreach (var eventOfMatch in eventsList ?? new List<EventsOfMatches>())
            {
                SummaryEventOfMatch summaryEventOfMatch = new SummaryEventOfMatch()
                {
                    EventOfMatch = eventOfMatch,
                    HomeTeam = eventOfMatch.EventTeam == matchDetails.Match.HomeTeam ? eventOfMatch.EventTeam : null,
                    AwayTeam = eventOfMatch.EventTeam == matchDetails.Match.AwayTeam ? eventOfMatch.EventTeam : null,
                    EventMinute = eventOfMatch.EventMinute,
                    EventName = eventOfMatch.EventName,
                    HomeTeamScore = 0,
                    AwayTeamScore = 0
                };
                if (IsGoal(eventOfMatch))
                {
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == matchDetails.Match.HomeTeam.ToUpper().Trim())
                    {
                        homeScore += 1;
                    }
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == matchDetails.Match.AwayTeam.ToUpper().Trim())
                    {
                        awayScore += 1;
                    }
                }
                summaryEventOfMatch.HomeTeamScore = homeScore;
                summaryEventOfMatch.AwayTeamScore = awayScore;
                summaryEventOfMatchesList.Add(summaryEventOfMatch);
            }
            summaryEventOfMatchesList.Reverse();
            return summaryEventOfMatchesList;
        }

        internal Statistic GetStatistics(List<EventsOfMatches> eventsDataList, Matches match)
        {
            Statistic statistic = new Statistic();
            foreach (EventsOfMatches matchEvent in eventsDataList)
            {
                if (matchEvent?.EventTeam?.ToLower().Trim() == match?.HomeTeam?.ToLower().Trim())
                {
                    AddTeamStatistic(matchEvent, statistic.HomeTeamStatistic);
                }
                else if (matchEvent?.EventTeam?.ToLower().Trim() == match?.AwayTeam?.ToLower().Trim())
                {
                    AddTeamStatistic(matchEvent, statistic.AwayTeamStatistic);
                }
            }

            if (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal != 0)
            {
                statistic.HomeTeamStatistic.GoalProcent = statistic.HomeTeamStatistic.Goal * 100 / (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal);
                statistic.AwayTeamStatistic.GoalProcent = statistic.AwayTeamStatistic.Goal * 100 / (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal);
            }
            if (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard != 0)
            {
                statistic.HomeTeamStatistic.YellowCardProcent = statistic.HomeTeamStatistic.YellowCard * 100 / (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard);
                statistic.AwayTeamStatistic.YellowCardProcent = statistic.AwayTeamStatistic.YellowCard * 100 / (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard);
            }
            if (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard != 0)
            {
                statistic.HomeTeamStatistic.RedCardProcent = statistic.HomeTeamStatistic.RedCard * 100 / (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard);
                statistic.AwayTeamStatistic.RedCardProcent = statistic.AwayTeamStatistic.RedCard * 100 / (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard);
            }
            if (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner != 0)
            {
                statistic.HomeTeamStatistic.CornerProcent = statistic.HomeTeamStatistic.Corner * 100 / (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner);
                statistic.AwayTeamStatistic.CornerProcent = statistic.AwayTeamStatistic.Corner * 100 / (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner);
            }
            if (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget != 0)
            {
                statistic.HomeTeamStatistic.OnTargetProcent = statistic.HomeTeamStatistic.OnTarget * 100 / (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget);
                statistic.AwayTeamStatistic.OnTargetProcent = statistic.AwayTeamStatistic.OnTarget * 100 / (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget);
            }
            if (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget != 0)
            {
                statistic.HomeTeamStatistic.OffTargetProcent = statistic.HomeTeamStatistic.OffTarget * 100 / (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget);
                statistic.AwayTeamStatistic.OffTargetProcent = statistic.AwayTeamStatistic.OffTarget * 100 / (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget);
            }
            if (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot != 0)
            {
                statistic.HomeTeamStatistic.BlockedShotProcent = statistic.HomeTeamStatistic.BlockedShot * 100 / (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot);
                statistic.AwayTeamStatistic.BlockedShotProcent = statistic.AwayTeamStatistic.BlockedShot * 100 / (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot);
            }
            if (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance != 0)
            {
                statistic.HomeTeamStatistic.BigChanceProcent = statistic.HomeTeamStatistic.BigChance * 100 / (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance);
                statistic.AwayTeamStatistic.BigChanceProcent = statistic.AwayTeamStatistic.BigChance * 100 / (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance);
            }


            return statistic;
        }

        private void AddTeamStatistic(EventsOfMatches matchEvent, TeamStatistic teamStatistic)
        {
            switch (matchEvent?.EventName?.ToLower())
            {
                case "goal":
                case "penaltyscored":
                    teamStatistic.Goal += 1;
                    teamStatistic.OnTarget += 1;
                    break;
                case "owngoal":
                    teamStatistic.Goal += 1;
                    break;
                case "yellow":
                    teamStatistic.YellowCard += 1;
                    break;
                case "red":
                    teamStatistic.RedCard += 1;
                    break;
                case "corner":
                    teamStatistic.Corner += 1;
                    break;
                case "ontarget":
                    teamStatistic.OnTarget += 1;
                    break;
                case "offtarget":
                    teamStatistic.OffTarget += 1;
                    break;
                case "blockedshot":
                    teamStatistic.BlockedShot += 1;
                    break;
                case "bigchance":
                    teamStatistic.BigChance += 1;
                    break;
            }
        }

    }
}