using DystirWeb.Models;
using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DystirWeb.Controllers
{
    [Route("data/request/datapackage")]
    [ApiController]
    public class DataPackageController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly DystirService _dystirService;
        private readonly StandingService _standingService;
        private readonly StatisticCompetitionsService _statisticCompetitionsService;

        public DataPackageController(AuthService authService, DystirService dystirService, StandingService standingService, StatisticCompetitionsService statisticCompetitionsService)
        {
            _authService = authService;
            _dystirService = dystirService;
            _standingService = standingService;
            _statisticCompetitionsService = statisticCompetitionsService;
        }

        // GET: data/request/datapackage/royn
        [HttpGet("{requestorValue}")]
        public IActionResult GetNumberOfMatches(string requestorValue)
        {
            var dataPackage = new DataPackage();
            if (_authService.IsAuthorizedRequestor(requestorValue))
            {
                if (requestorValue.ToLower() == "royn")
                {
                    dataPackage.Matches = GetMatchesListForRoyn();
                    var competitions = new List<string>() { "Betri deildin kvinnur", "2. deild" };
                    dataPackage.Standings = _standingService.GetStandings().Where(x => competitions.Contains(x.StandingCompetitionName)).ToList();
                    dataPackage.FullPlayerStatistics = new List<PlayerStatistics>();
                    var statistics = _statisticCompetitionsService.GetCompetitionsStatistic().Where(x => competitions.Contains(x.CompetitionName)).ToList();
                    foreach (var statistic in statistics)
                    {
                        var statisticOfPlayers = new PlayerStatistics(statistic);
                        dataPackage.FullPlayerStatistics.Add(statisticOfPlayers);
                    }
                }
            }
            return Ok(dataPackage);
        }

        private List<MatchFullTime> GetMatchesListForRoyn()
        {
            var matchesFullTime = new List<MatchFullTime>();
            var date = new DateTime(DateTime.Now.Year, 1, 1);
            var matchesList = _dystirService.AllMatches?
                .Where(x => x.Time.Value.Date > date && (x.MatchTypeID == 6 || x.MatchTypeID == 101))
                .OrderBy(x => x.MatchTypeID)
                .ThenBy(x => x.Time)
                .ThenBy(x => x.MatchID)?.ToList();
            foreach (var match in matchesList ?? new List<Matches>())
            {
                matchesFullTime.Add(new MatchFullTime(match));
            }

            return matchesFullTime;
        }
    }

    internal class DataPackage
    {
        public List<MatchFullTime> Matches { get; internal set; }
        public List<Standing> Standings { get; internal set; }
        public List<PlayerStatistics> FullPlayerStatistics { get; internal set; }
    }

    internal class PlayerStatistics
    {
        public List<GoalScorers> GoalScorers { get; private set; }
        public string CompetitionName { get; private set; }

        public PlayerStatistics(CompetitionStatistic statistic)
        {
            CompetitionName = statistic.CompetitionName;
            var goalScorers = new List<GoalScorers>();
            foreach (var player in statistic.GoalPlayers)
            {
                var goalScorer = new GoalScorers(player);
                goalScorers.Add(goalScorer);
            }
            GoalScorers = goalScorers.OrderByDescending(x => x.Goals).ToList();
        }
    }

    internal class GoalScorers
    {
        public string Name { get; private set; }
        public int Goals { get; private set; }

        public GoalScorers(PlayersOfMatches player)
        {
            Name = player.FirstName;
            Goals = player.Goal ?? 0;
        }
    }
}