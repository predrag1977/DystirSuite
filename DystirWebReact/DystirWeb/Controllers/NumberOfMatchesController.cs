using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DystirWeb.Controllers
{
    [Route("data/request/[controller]")]
    [ApiController]
    public class NumberOfMatchesController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly DystirService _dystirService;

        public NumberOfMatchesController(AuthService authService, DystirService dystirService)
        {
            _authService = authService;
            _dystirService = dystirService;
        }

        // GET: data/request/numberofmatches/portal
        [HttpGet("{requestorValue}")]
        public IActionResult GetNumberOfMatches(string requestorValue)
        {
            string value = "";
            if (_authService.IsAuthorizedRequestor(requestorValue))
            {
                var matchesCount = GetMatchesList().Count;
                if (requestorValue.Equals("portal", StringComparison.CurrentCultureIgnoreCase))
                {
                    matchesCount = GetMatchesListForPortal().Count;
                }
                value = string.Format("{0}:{1}", GetMatchesList().FirstOrDefault()?.MatchTypeName ?? "Competition", matchesCount.ToString());
            }
            return Ok(value);
        }

        private List<Matches> GetMatchesList()
        {
            var fromDate = DateTime.UtcNow.Date.AddDays(0);
            var toDate = fromDate.AddDays(0);
            var matchesList = _dystirService.AllMatches?
                .Where(x => x.Time.Value.Date >= fromDate && x.Time.Value.Date <= toDate)
                .OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).ThenBy(x => x.MatchID)?.ToList();

            return matchesList ?? new List<Matches>();
        }

        private List<Matches> GetMatchesListForPortal()
        {
            var fromDate = DateTime.UtcNow.Date.AddDays(0);
            var toDate = fromDate.AddDays(0);
            var matchesList = _dystirService.AllMatches?
                .Where(x => x.Time.Value.Date >= fromDate
                && x.Time.Value.Date <= toDate
                && x.MatchTypeID != 5 && x.MatchTypeID != 6)
                .OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).ThenBy(x => x.MatchID)?.ToList();

            return matchesList ?? new List<Matches>();
        }

    }
}