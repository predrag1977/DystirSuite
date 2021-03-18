using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.ApiControllers;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;

namespace DystirWeb.Controllers
{
    
    public class StandingsViewController : Controller
    {
        private static IEnumerable<Standing> listStandings;

        private DystirDBContext db;

        public StandingsViewController(DystirDBContext dystirDBContext)
        {
            db = dystirDBContext;
        }
        
        [Route("Støðan")]
        public IActionResult Index()
        {
            listStandings = new StandingsController(db).Get();
            return View(listStandings);
        }

        public IActionResult MatchesList(string selectedCompetition)
        {
            selectedCompetition = string.IsNullOrWhiteSpace(selectedCompetition) ? listStandings?.FirstOrDefault().StandingCompetitionName ?? string.Empty : selectedCompetition;
            Standing standing = listStandings.FirstOrDefault(x => x.StandingCompetitionName == selectedCompetition);
            StandingsModelView standingsModelView = new StandingsModelView()
            {
                SelectedCompetition = selectedCompetition,
                CompetitionsList = GetCompetitionsList(listStandings),
                Standing = standing
            };
            return PartialView("~/Views/PartialViews/StandingsPartialView.cshtml", standingsModelView);
        }

        private IEnumerable<string> GetCompetitionsList(IEnumerable<Standing> listStandings)
        {
            List<string> competitionsList = new List<string>();
            foreach (var standing in listStandings ?? new List<Standing>())
            {
                competitionsList.Add(standing.StandingCompetitionName);
            }
            return competitionsList;
        }
    }
}