using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DystirWeb.Controllers
{
    [Route("data/request/matchesfulltime")]
    [ApiController]
    public class MatchesFullTimeController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly DystirService _dystirService;

        public MatchesFullTimeController(AuthService authService, DystirService dystirService)
        {
            _authService = authService;
            _dystirService = dystirService;
        }

        // GET: data/request/matchesfulltime/royn
        [HttpGet("{requestorValue}")]
        public IActionResult GetNumberOfMatches(string requestorValue)
        {
            var matches = new List<MatchFullTime>();
            if (_authService.IsAuthorizedRequestor(requestorValue))
            {
                matches = GetMatchesListForRoyn();
            }
            return Ok(matches);
        }

        private List<MatchFullTime> GetMatchesListForRoyn()
        {
            var matchesFullTime = new List<MatchFullTime>();
            var date = DateTime.Now.AddHours(0).Date;
            var matchesList = _dystirService.AllMatches?
                .Where(x => x.Time.Value.Date == date && x.MatchTypeID != 5 && x.MatchTypeID != 6)
                .OrderBy(x => x.MatchTypeID)
                .ThenBy(x => x.Time)
                .ThenBy(x => x.MatchID)?.ToList();
            foreach(var match in matchesList ?? new List<Matches>())
            {
                matchesFullTime.Add(new MatchFullTime(match));
            }

            return matchesFullTime;
        }
    }
}