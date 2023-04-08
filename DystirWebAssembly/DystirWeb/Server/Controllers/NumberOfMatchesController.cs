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
                //return BadRequest(new UnauthorizedAccessException().Message);

                //foreach (var matchGroup in GetMatches())
                //{
                //    value += String.Format("{0}:{1}\n", matchGroup.Key, matchGroup.Count());
                //}
                //if (GetMatches().Count() == 0)
                //{
                //    value = "Competition:0";
                //}
                var matchesCount = (GetMatchesList() ?? new List<Matches>()).Count;
                //matchesCount = matchesCount > 5 ? 5 : matchesCount;
                value = String.Format("{0}:{1}", GetMatchesList().FirstOrDefault()?.MatchTypeName ?? "Competition", matchesCount.ToString());
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

        private IEnumerable<IGrouping<string, Matches>> GetMatches()
        {
            var fromDate = DateTime.Now.Date.AddDays(0);
            var toDate = fromDate.AddDays(3);
            var matchesListGroup = _dystirService.AllMatches?
                .OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).ThenBy(x => x.MatchID)
                .Where(x => x.Time.Value.Date >= fromDate && x.Time.Value.Date <= toDate)
                .GroupBy(x => x.MatchTypeName)?.ToList();

            return matchesListGroup ?? new List<IGrouping<string, Matches>>();
        }
    }
}