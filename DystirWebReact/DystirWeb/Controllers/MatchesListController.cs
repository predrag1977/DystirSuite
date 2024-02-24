using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DystirWeb.Controllers
{
    [Route("data/request/matches")]
    [ApiController]
    public class MatchesListController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly DystirService _dystirService;

        public MatchesListController(AuthService authService, DystirService dystirService)
        {
            _authService = authService;
            _dystirService = dystirService;
        }

        // GET: data/request/matches/portal
        [HttpGet("{requestorValue}")]
        public IActionResult GetNumberOfMatches(string requestorValue)
        {
            var matches = new List<Matches>();
            if (_authService.IsAuthorizedRequestor(requestorValue))
            {
                if(requestorValue.Equals("roysni", StringComparison.CurrentCultureIgnoreCase))
                {
                    matches = GetMatchesListForPortal();
                }
                else
                {
                    matches = GetMatchesListForPortal();
                }
                
            }
            return Ok(matches);
        }

        private List<Matches> GetMatchesListForPortal()
        {
            var date = DateTime.UtcNow.AddHours(0).Date;
            var matchesList = _dystirService.AllMatches?
                .Where(x => x.Time.Value.Date == date && x.MatchTypeID != 5 && x.MatchTypeID != 6)
                .OrderBy(x => x.MatchTypeID)
                .ThenBy(x => x.Time)
                .ThenBy(x => x.MatchID)?.ToList();

            return matchesList ?? new List<Matches>();
        }

        private List<Matches> GetMatchesListForRoysni()
        {
            var fromDate = DateTime.UtcNow.AddDays(-2).AddHours(0).Date;
            var toDate = fromDate.AddDays(5).Date;
            var matchesList = _dystirService.AllMatches?
                .Where(x => x.Time.Value.Date > fromDate && x.Time.Value.Date < toDate  && x.MatchTypeID != 5 && x.MatchTypeID != 6)
                .OrderBy(x => x.MatchTypeID)
                .ThenBy(x => x.Time)
                .ThenBy(x => x.MatchID)?.ToList();

            return matchesList ?? new List<Matches>();
        }
    }
}