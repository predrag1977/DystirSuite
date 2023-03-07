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

        // GET: data/request/NumberOfMatches/portal
        [HttpGet("{requestorValue}")]
        public IActionResult GetNumberOfMatches(string requestorValue)
        {
            string value = "";
            if (_authService.IsAuthorizedRequestor(requestorValue))
            {
                //return BadRequest(new UnauthorizedAccessException().Message);

                var matchesCompetitionList = new List<string>();
                foreach (var matchGroup in GetMatches())
                {
                    value += String.Format("{0}:{1}\n", matchGroup.Key, matchGroup.Count());
                }
                if (GetMatches().Count() == 0)
                {
                    value = "Competition:0";
                }
            }
            return Ok(value);
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