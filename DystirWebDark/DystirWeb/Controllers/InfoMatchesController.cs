using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DystirWeb.ApiControllers;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DystirWeb.Controllers
{
    public class InfoMatchesController : Controller
    {
        private DystirDBContext dbContext;

        public InfoMatchesController(DystirDBContext dystirDBContext)
        {
            dbContext = dystirDBContext;
        }

        public IActionResult Index()
        {
            List<Matches> matchesList = GetMatches();
            IEnumerable<IGrouping<string, Matches>> matchGroup = matchesList.GroupBy(x => x.MatchTypeName);
            return View(matchGroup);
        }

        private List<Matches> GetMatches()
        {
            List<Matches> matchesList = new List<Matches>();
            try
            {
                var fromDate = DateTime.UtcNow.Date.AddDays(-30);
                var toDate = DateTime.UtcNow.Date.AddDays(60);
                matchesList = dbContext.Matches?.Where(x=>x.MatchTypeId == 1 || x.MatchTypeId == 101 || x.MatchTypeId == 2 
                || x.MatchTypeId == 102 || x.MatchTypeId == 5 || x.MatchTypeId == 4 || x.MatchTypeId == 12 
                || x.MatchTypeId == 13 || x.MatchTypeId == 301 || x.MatchTypeId == 302 
                || x.MatchTypeId == 303 || x.MatchTypeId == 16 || x.MatchTypeId == 0 || x.MatchTypeId == 40)
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

        public IActionResult LoadMatches()
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

        public IActionResult InfoMatchesList()
        {
            IEnumerable<IGrouping<string, Matches>> matchesGroup = GetMatches()?.GroupBy(x => x.MatchTypeName);
            MatchesModelView matchesModelView = new MatchesModelView()
            {
                CompetitionsList = GetCompetitionsList(matchesGroup),
                MatchesGroups = matchesGroup
            };
            return PartialView("~/Views/PartialViews/InfoMatchesPartialView.cshtml", matchesModelView);
        }
        
        public IActionResult GetInfoMatchesListView(string matchesList)
        {
            IEnumerable<IGrouping<string, Matches>> matchesGroup = GetMatchesFromJson(matchesList)?.GroupBy(x => x.MatchTypeName);
            MatchesModelView matchesModelView = new MatchesModelView()
            {
                CompetitionsList = GetCompetitionsList(matchesGroup),
                MatchesGroups = matchesGroup
            };
            return PartialView("~/Views/PartialViews/InfoMatchesPartialView.cshtml", matchesModelView);
        }

        private List<Matches> GetMatchesFromJson(string matchesListJson)
        {
            List<Matches> matchesList = new List<Matches>();
            try
            {
                List<Matches> allMatchesList = JsonConvert.DeserializeObject<List<Matches>>(matchesListJson);
                var fromDate = DateTime.UtcNow.Date.AddDays(-5);
                var toDate = DateTime.UtcNow.Date.AddDays(1);
                matchesList = allMatchesList?.Where(x => x.MatchTypeId == 1 || x.MatchTypeId == 101 || x.MatchTypeId == 2
                || x.MatchTypeId == 102 || x.MatchTypeId == 5 || x.MatchTypeId == 12
                || x.MatchTypeId == 13 || x.MatchTypeId == 301 || x.MatchTypeId == 302
                || x.MatchTypeId == 303 || x.MatchTypeId == 16 || x.MatchTypeId == 0 || x.MatchTypeId == 40)
                    .OrderBy(x => x.MatchTypeId)
                    .ThenBy(x => x.Time)
                    //.ThenBy(x => x.StatusId)
                    //.Where(x => x.Time.Value.Date.Date >= fromDate && x.Time.Value.Date.Date <= toDate 
                    .Where(x => x.Time.Value.Date.Date == DateTime.UtcNow.Date
                    && x.MatchActivation != 1 && x.MatchActivation != 2).ToList();
            }
            catch (Exception)
            {

            }
            return matchesList;
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
    }
}