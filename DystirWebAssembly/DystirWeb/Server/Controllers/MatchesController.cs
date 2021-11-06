using System;
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
        private readonly DystirService _dystirService;
        private readonly MatchDetailsService _matchDetailsService;
        private DystirDBContext _dystirDBContext;

        public MatchesController (IHubContext<DystirHub> hubContext,
            DystirDBContext dystirDBContext,
            DystirService dystirService,
            MatchDetailsService matchDetailsService)
        {
            _hubContext = hubContext;
            _dystirDBContext = dystirDBContext;
            _dystirService = dystirService;
            _matchDetailsService = matchDetailsService;
        }

        // GET: api/Matches
        [HttpGet]
        public async Task<IEnumerable<Matches>> GetMatches(string action)
        {
            Debug.WriteLine("Start:" + DateTime.Now.ToString("hh:mm:ss:ff"));
            IEnumerable<Matches> matches;
            switch (action?.ToLower())
            {
                case "matches":
                    var fromDate = DateTime.UtcNow.Date.AddDays(-15);
                    var toDate = DateTime.UtcNow.Date.AddDays(15);
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
                    int year = DateTime.UtcNow.Year;
                    fromDate = new DateTime(year, 1, 1);
                    matches = _dystirService.AllMatches?.Where(y => y.MatchActivation != 1 && y.MatchActivation != 2 && y.Time > fromDate);
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
        [HttpPut("{id}")]
        public IActionResult PutMatches(int id, [FromBody] Matches matches)
        {
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
                _dystirDBContext.Entry(matchInDB).CurrentValues.SetValues(matches);
                _dystirDBContext.Entry(matchInDB).State = EntityState.Modified;
                _dystirDBContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            HubSend(matches);

            return StatusCode(StatusCodes.Status204NoContent);
        }

        // POST: api/Matches
        [HttpPost]
        public IActionResult PostMatches([FromBody] Matches matches)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                matches.StatusTime = DateTime.UtcNow;
                //matches.Time = matches.Time.Value.ToUniversalTime();
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
        [HttpDelete("{id}")]
        public IActionResult DeleteMatches(int id)
        {
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
            return _dystirDBContext.Matches.Count(e => e.MatchID == id) > 0;
        }
        private void HubSend(Matches match)
        {
            HubSender hubSender = new HubSender();
            hubSender.SendMatch(_hubContext, match);
            HubSendMatchDetails(hubSender, match);
        }

        private void HubSendMatchDetails(HubSender hubSender, Matches match)
        {
            MatchDetails matchDetails = _matchDetailsService.GetMatchDetails(match.MatchID, true);
            matchDetails.Match = match;
            _dystirService.UpdateDataAsync(matchDetails);
            hubSender.SendMatchDetails(_hubContext, matchDetails);
        }
    }
}