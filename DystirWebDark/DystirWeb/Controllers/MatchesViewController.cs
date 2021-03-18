using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;

namespace DystirWeb.Controllers
{
    public class MatchesViewController : Controller
    {
        private DystirDBContext dbContext;

        public MatchesViewController(DystirDBContext dystirDBContext)
        {
            dbContext = dystirDBContext;
        }

        public IActionResult IndexAsync()
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
                var fromDate = DateTime.UtcNow.Date.AddDays(-5);
                var toDate = DateTime.UtcNow.Date.AddDays(5);
                matchesList = dbContext.Matches?.OrderBy(x => x.MatchTypeId).ThenByDescending(x => x.Time)
                    .Where(x => x.Time.Value.Date.Date >= fromDate && x.Time.Value.Date.Date <= toDate && x.MatchActivation != 1 && x.MatchActivation != 2).ToList();
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

        public IActionResult MatchesListAsync(string selectedDate, string totalMinutes)
        {
            List<Matches> matchesList = GetMatches();
            double diffrentHours = double.Parse(totalMinutes, new CultureInfo("da-DK")) / 60;
            if (string.IsNullOrEmpty(selectedDate))
            {
                selectedDate = DateTime.UtcNow.AddHours(-diffrentHours).ToString("d", new CultureInfo("da-DK"));
            }
            DateTime date = Convert.ToDateTime(selectedDate, new CultureInfo("da-DK")).Date;

            var newMatchesList = new List<Matches>();
            if (date != DateTime.MinValue)
            {
                newMatchesList = matchesList.Where(x => x.Time.Value.Date == date).OrderBy(x => x.MatchTypeId).ThenBy(x => x.Time).ToList();
            }

            IEnumerable<IGrouping<string, Matches>> matchGroup = newMatchesList.GroupBy(x => x.MatchTypeName);
            MatchesModelView matchesModelView = new MatchesModelView()
            {
                SelectedDate = date,
                MatchesGroups = matchGroup
            };
            return PartialView("~/Views/PartialViews/MatchesPartialView.cshtml", matchesModelView);
        }


        public IActionResult GetMatchesListView(string selectedDate, string totalMinutes, string matchesList)
        {
            List<Matches> allMatchesFromJson = JsonConvert.DeserializeObject<List<Matches>>(matchesList);
            List<Matches> allMatchesList = GetMatchesFromJson(allMatchesFromJson);
            double diffrentHours = double.Parse(totalMinutes, new CultureInfo("da-DK")) / 60;
            if (string.IsNullOrEmpty(selectedDate))
            {
                selectedDate = DateTime.UtcNow.AddHours(-diffrentHours).ToString("d", new CultureInfo("da-DK"));
            }
            DateTime date = Convert.ToDateTime(selectedDate, new CultureInfo("da-DK")).Date;

            var newMatchesList = new List<Matches>();
            if (date != DateTime.MinValue)
            {
                newMatchesList = allMatchesList.Where(x => x.Time.Value.Date == date).OrderBy(x => x.MatchTypeId).ThenBy(x => x.Time).ToList();
            }

            IEnumerable<IGrouping<string, Matches>> matchGroup = newMatchesList.GroupBy(x => x.MatchTypeName);
            MatchesModelView matchesModelView = new MatchesModelView()
            {
                SelectedDate = date,
                MatchesGroups = matchGroup
            };
            return PartialView("~/Views/PartialViews/MatchesPartialView.cshtml", matchesModelView);
        }

        private List<Matches> GetMatchesFromJson(List<Matches> allMatches)
        {
            List<Matches> matchesList = new List<Matches>();
            try
            {
                var fromDate = DateTime.UtcNow.Date.AddDays(-5);
                var toDate = DateTime.UtcNow.Date.AddDays(5);
                matchesList = allMatches?.OrderBy(x => x.MatchTypeId).ThenByDescending(x => x.Time)
                    .Where(x => x.Time.Value.Date.Date >= fromDate && x.Time.Value.Date.Date <= toDate && x.MatchActivation != 1 && x.MatchActivation != 2).ToList();
            }
            catch (Exception)
            {

            }
            return matchesList;
        }
    }
}