using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DystirWeb.Controllers
{
    [Route("data/request/[controller]")]
    [ApiController]
    public class CountMatchesController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly DystirService _dystirService;

        public CountMatchesController(AuthService authService, DystirService dystirService)
        {
            _authService = authService;
            _dystirService = dystirService;
        }

        // GET: data/request/countmatches/info
        [HttpGet("{requestorValue}")]
        public IActionResult GetCountMatches(string requestorValue)
        {
            string value = "";
            if (_authService.IsAuthorizedRequestor(requestorValue))
            {
                var matchesCount = (GetMatchesList() ?? new List<Matches>()).Count;
                value = matchesCount.ToString();
            }
            return Ok(value);
        }

        private List<Matches> GetMatchesList()
        {
            var fromDate = DateTime.Now.AddHours(1).Date.AddDays(0);
            var toDate = fromDate.AddDays(0);
            var matchesList = _dystirService.AllMatches?
                .OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).ThenBy(x => x.MatchID)
                .Where(x => x.Time.Value.Date >= fromDate && x.Time.Value.Date <= toDate)?.ToList();

            return matchesList ?? new List<Matches>();
        }
    }
}