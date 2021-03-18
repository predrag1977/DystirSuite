using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DystirWeb.Controllers
{
    public class ResultsController : Controller
    {
        private static List<Matches> _matchesList;

        private DystirDBContext dbContext;

        public ResultsController(DystirDBContext dystirDBContext)
        {
            dbContext = dystirDBContext;
        }

        [Route("Úrslit")]
        public IActionResult Index()
        {
            LoadResults();
            IEnumerable<IGrouping<string, Matches>> matchesGroup = _matchesList?.GroupBy(x => x.MatchTypeName);
            return View(matchesGroup);
        }

        public IActionResult LoadResults()
        {
            try
            {
                _matchesList = new List<Matches>();
                int year = DateTime.UtcNow.Year;
                var fromDate = new DateTime(year, 1, 1);
                var toDate = DateTime.UtcNow.Date.AddDays(3);
                _matchesList = dbContext.Matches?.OrderBy(x => x.MatchTypeId).ThenByDescending(x => x.Time)
                    .Where(x => x.Time.Value.Date.Date > fromDate
                    && x.Time.Value.Date.Date < toDate
                    && (x.StatusId == 13 || x.StatusId == 12)).ToList();
            }
            catch
            {
                return Json(false);
            }
            return Json(true);
        }
        
        public IActionResult MatchesList(string selectedCompetition)
        {
            IEnumerable<IGrouping<string, Matches>> matchesGroup = _matchesList?.GroupBy(x => x.MatchTypeName);
            selectedCompetition = string.IsNullOrWhiteSpace(selectedCompetition) ? matchesGroup?.FirstOrDefault()?.Key ?? string.Empty : selectedCompetition;
            var selectedMatchesGroup = matchesGroup?.Where(x => x.Key == selectedCompetition);
            MatchesModelView matchesModelView = new MatchesModelView()
            {
                SelectedCompetition = selectedCompetition?.Trim(),
                CompetitionsList = GetCompetitionsList(matchesGroup),
                MatchesGroups = selectedMatchesGroup
            };
            return PartialView("~/Views/PartialViews/ResultsPartialView.cshtml", matchesModelView);
        }

        private IEnumerable<string> GetCompetitionsList(IEnumerable<IGrouping<string, Matches>> matchGroups)
        {
            List<string> competitionsList = new List<string>();
            foreach (var matchGroup in matchGroups ?? new List<IGrouping<string, Matches>>())
            {
                competitionsList.Add(matchGroup.Key);
            }
            return competitionsList;
        }

        public IActionResult GetMatchesListView(string selectedCompetition, string matchesList)
        {
            List<Matches> allMatches = JsonConvert.DeserializeObject<List<Matches>>(matchesList);
            IEnumerable<IGrouping<string, Matches>> matchesGroup = GetResults(allMatches)?.GroupBy(x => x.MatchTypeName);
            selectedCompetition = string.IsNullOrWhiteSpace(selectedCompetition) ? matchesGroup?.FirstOrDefault()?.Key ?? string.Empty : selectedCompetition;
            var selectedMatchesGroup = matchesGroup?.Where(x => x.Key == selectedCompetition);
            MatchesModelView matchesModelView = new MatchesModelView()
            {
                SelectedCompetition = selectedCompetition?.Trim(),
                CompetitionsList = GetCompetitionsList(matchesGroup),
                MatchesGroups = selectedMatchesGroup
            };
            return PartialView("~/Views/PartialViews/ResultsPartialView.cshtml", matchesModelView);
        }

        public List<Matches> GetResults(List<Matches> allMatches)
        {
            List<Matches> matchesList = new List<Matches>();
            try
            {
                int year = DateTime.UtcNow.Year;
                var fromDate = new DateTime(year, 1, 1);
                var toDate = DateTime.UtcNow.Date.AddDays(3);
                matchesList = dbContext.Matches?.OrderBy(x => x.MatchTypeId).ThenByDescending(x => x.Time)
                    .Where(x => x.Time.Value.Date.Date > fromDate
                    && x.Time.Value.Date.Date < toDate
                    && (x.StatusId == 13 || x.StatusId == 12)).ToList();
            }
            catch
            {

            }
            return matchesList;
        }
    }
}