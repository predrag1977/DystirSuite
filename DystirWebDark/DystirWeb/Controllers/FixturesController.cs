using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DystirWeb.Controllers
{
    public class FixturesController : Controller
    {
        private static List<Matches> _matchesList;

        private DystirDBContext dbContext;

        public FixturesController(DystirDBContext dystirDBContext)
        {
            dbContext = dystirDBContext;
        }

        [Route("KomandiDystir")]
        public IActionResult Index()
        {
            LoadFixtures();
            IEnumerable<IGrouping<string, Matches>> matchesGroup = _matchesList?.GroupBy(x => x.MatchTypeName);
            return View(matchesGroup);
        }

        public IActionResult LoadFixtures()
        {
            try
            {
                int year = DateTime.UtcNow.Year;
                var fromDate = DateTime.UtcNow.Date.AddDays(-3);
                var toDate = new DateTime(year + 1, 1, 1);
                _matchesList = dbContext.Matches?.OrderBy(x => x.MatchTypeId).ThenBy(x => x.Time)
                    .Where(x => x.Time.Value.Date.Date > fromDate
                    && x.Time.Value.Date.Date < toDate
                    && (x.StatusId < 2 || x.StatusId > 13)).ToList();
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
            return PartialView("~/Views/PartialViews/FixturesPartialView.cshtml", matchesModelView);
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
            var allMatches = JsonConvert.DeserializeObject<List<Matches>>(matchesList);
            IEnumerable<IGrouping<string, Matches>> matchesGroup = GetFixtures(allMatches)?.GroupBy(x => x.MatchTypeName);
            selectedCompetition = string.IsNullOrWhiteSpace(selectedCompetition) ? matchesGroup?.FirstOrDefault()?.Key ?? string.Empty : selectedCompetition;
            var selectedMatchesGroup = matchesGroup?.Where(x => x.Key == selectedCompetition);
            MatchesModelView matchesModelView = new MatchesModelView()
            {
                SelectedCompetition = selectedCompetition?.Trim(),
                CompetitionsList = GetCompetitionsList(matchesGroup),
                MatchesGroups = selectedMatchesGroup
            };
            return PartialView("~/Views/PartialViews/FixturesPartialView.cshtml", matchesModelView);
        }

        public List<Matches> GetFixtures(List<Matches> allMatches)
        {
            List<Matches> matchesList = new List<Matches>();
            try
            {
                int year = DateTime.UtcNow.Year;
                var fromDate = DateTime.UtcNow.Date.AddDays(-3);
                var toDate = new DateTime(year + 1, 1, 1);
                matchesList = allMatches?.OrderBy(x => x.MatchTypeId).ThenBy(x => x.Time)
                    .Where(x => x.Time.Value.Date.Date > fromDate
                    && x.Time.Value.Date.Date < toDate
                    && (x.StatusId < 2 || x.StatusId > 13)).ToList();
            }
            catch
            {

            }
            return matchesList;
        }
    }
}