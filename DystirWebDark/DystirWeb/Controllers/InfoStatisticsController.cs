using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.ApiControllers;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;

namespace DystirWeb.Controllers
{
    
    public class InfoStatisticsController : Controller
    {
        private static IEnumerable<CompetitionStatistic> _listCompetitionStatistic;

        private DystirDBContext db;

        public InfoStatisticsController(DystirDBContext dystirDBContext)
        {
            db = dystirDBContext;
        }
        public IActionResult Index()
        {
            _listCompetitionStatistic = new StatisticsController(db).Get();
            return View(_listCompetitionStatistic);
        }

        public IActionResult StatisticsList(string selectedCompetition)
        {
            selectedCompetition = string.IsNullOrWhiteSpace(selectedCompetition) ? _listCompetitionStatistic?.FirstOrDefault().CompetitionName ?? string.Empty : selectedCompetition;
            CompetitionStatistic competitionStatistic = _listCompetitionStatistic.FirstOrDefault(x => x.CompetitionName == selectedCompetition);
            FullStatisticModelView fullStatisticModelView = new FullStatisticModelView()
            {
                SelectedCompetition = selectedCompetition,
                CompetitionsList = GetCompetitionsList(),
                CompetitionStatistic = competitionStatistic
            };
            return PartialView("~/Views/PartialViews/StatisticsPartialView.cshtml", fullStatisticModelView);
        }

        private IEnumerable<string> GetCompetitionsList()
        {
            List<string> competitionsList = new List<string>();
            foreach (var statistic in _listCompetitionStatistic ?? new List<CompetitionStatistic>())
            {
                competitionsList.Add(statistic.CompetitionName);
            }
            return competitionsList;
        }
    }
}