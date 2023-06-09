using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DystirWeb.Server.DystirDB;
using DystirWeb.Server.Hubs;
using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IHubContext<DystirHub> _hubContext;
        private readonly AuthService _authService;
        private readonly DystirService _dystirService;
        private readonly MatchDetailsService _matchDetailsService;
        private DystirDBContext _dystirDBContext;

        public MatchesController (AuthService authService,
            DystirService dystirService,
            MatchDetailsService matchDetailsService,
            DystirDBContext dystirDBContext,
            IHubContext<DystirHub> hubContext)
        {
            _hubContext = hubContext;
            _authService = authService;
            _dystirService = dystirService;
            _dystirDBContext = dystirDBContext;
            _matchDetailsService = matchDetailsService;
        }

        // GET: api/Matches
        // GET: api/Matches
        [HttpGet]
        public async Task<IEnumerable<Matches>> GetMatches(string action)
        {
            Debug.WriteLine("Start:" + DateTime.Now.ToString("hh:mm:ss:ff"));
            IEnumerable<Matches> matches;
            int year = DateTime.UtcNow.Year;
            var fromDate = DateTime.UtcNow.Date.AddDays(-15);
            var toDate = DateTime.UtcNow.Date.AddDays(15);
            switch (action?.ToLower())
            {
                case "matches":
                    matches = _dystirService.AllMatches?.Where(x => x.Time > fromDate && x.Time < toDate && x.MatchActivation != 1 && x.MatchActivation != 2);
                    break;
                case "results":
                    matches = _dystirService.AllMatches?.Where(y => y.MatchActivation != 1 && y.MatchActivation != 2);
                    break;
                case "fixtures":
                    matches = _dystirService.AllMatches?.Where(y => y.MatchActivation != 1 && y.MatchActivation != 2);
                    break;
                case "archived":
                    matches = _dystirService.AllMatches?.Where(y => y.MatchActivation == 1);
                    break;
                default:
                    fromDate = new DateTime(year, 1, 1);
                    matches = _dystirService.AllMatches?.Where(y => y.Time > fromDate
                            && y.MatchActivation != 1
                            && y.MatchActivation != 2);
                    break;
            }
            Debug.WriteLine("Finished:" + DateTime.Now.ToString("hh:mm:ss:ff"));
            return await Task.FromResult(matches);
        }

        // GET: api/Matches/5
        [HttpGet("{id}", Name = "GetMatch")]
        public IActionResult GetMatches(int id)
        {
            Matches matches = _dystirDBContext.Matches.Find(id);
            if (matches == null)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // PUT: api/Matches/5
        [HttpPut("{id}/{token}")]
        public IActionResult PutMatches(int id, string token, [FromBody] Matches matches)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != matches.MatchID)
            {
                return BadRequest();
            }
            try
            {
                Matches matchInDB = _dystirDBContext.Matches.Find(id);
                if (!(matches.ExtraMinutes == 0 && matches.ExtraSeconds == 0) || matchInDB.StatusID != matches.StatusID)
                {
                    matches.StatusTime = DateTime.UtcNow.AddMinutes(-matches.ExtraMinutes).AddSeconds(-matches.ExtraSeconds);
                }
                matches.HomeTeamPenaltiesScore = matchInDB.HomeTeamPenaltiesScore;
                matches.AwayTeamPenaltiesScore = matchInDB.AwayTeamPenaltiesScore;
                
                _dystirDBContext.Entry(matchInDB).CurrentValues.SetValues(matches);
                _dystirDBContext.Entry(matchInDB).State = EntityState.Modified;
                _dystirDBContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!MatchesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw ex;
                }
            }
            HubSend(matches);

            return Ok(matches);
        }

        // POST: api/Matches
        [HttpPost("{token}")]
        public IActionResult PostMatches(string token, [FromBody] Matches matches)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                matches.StatusTime = DateTime.UtcNow;
                _dystirDBContext.Matches.Add(matches);
                _dystirDBContext.SaveChanges();

                HubSend(matches);
                IActionResult result = CreatedAtRoute("DefaultApi", new { id = matches.MatchID }, matches);
                return Ok("Successful");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // DELETE: api/Matches/5
        [HttpDelete("{id}/{token}")]
        public IActionResult DeleteMatches(int id, string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            Matches matches = _dystirDBContext.Matches.Find(id);
            if (matches == null)
            {
                return NotFound();
            }

            _dystirDBContext.Matches.Remove(matches);
            _dystirDBContext.SaveChanges();
            HubSend(matches);

            return Ok(matches);
        }

        private bool MatchesExists(int id)
        {
            return _dystirDBContext.Matches.Any(e => e.MatchID == id);
        }

        private void HubSend(Matches match)
        {
            HubSender hubSender = new HubSender();
            HubSender.SendMatch(_hubContext, match);
            HubSendMatchDetails(hubSender, match);
        }

        private void HubSendMatchDetails(HubSender hubSender, Matches match)
        {
            MatchDetails matchDetails = _matchDetailsService.GetMatchDetails(match.MatchID, true);
            matchDetails.Match = match;
            _dystirService.UpdateDataAsync(matchDetails);
            HubSender.SendMatchDetails(_hubContext, matchDetails);
        }
    }
}