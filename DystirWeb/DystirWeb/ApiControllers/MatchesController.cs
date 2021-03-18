using System;
using System.Linq;
using DystirWeb.ApiControllers;
using DystirWeb.Models;
using DystirWeb.ModelViews;
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
        private DystirDBContext _dystirDBContext;
        private readonly IHubContext<DystirHub> _hubContext;

        public MatchesController(IHubContext<DystirHub> hubContext, DystirDBContext dystirDBContext)
        {
            if (hubContext != null)
            {
                _hubContext = hubContext;
            }
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Matches
        [HttpGet]
        public IQueryable<Matches> GetMatches(string action)
        {
            IQueryable<Matches> matches;
            switch (action?.ToLower())
            {
                case "matches":
                    var fromDate = DateTime.UtcNow.Date.AddDays(-5);
                    var toDate = DateTime.UtcNow.Date.AddDays(5);
                    matches = _dystirDBContext.Matches.Where(x => x.Time > fromDate && x.Time < toDate && x.MatchActivation != 1 && x.MatchActivation != 2);
                    break;
                case "results":
                    matches = _dystirDBContext.Matches.Where(y => y.MatchActivation != 1 && y.MatchActivation != 2);
                    break;
                case "fixtures":
                    matches = _dystirDBContext.Matches.Where(y => y.MatchActivation != 1 && y.MatchActivation != 2);
                    break;
                case "archived":
                    matches = _dystirDBContext.Matches.Where(y => y.MatchActivation == 1);
                    break;
                default:
                    int year = DateTime.UtcNow.Year;
                    fromDate = new DateTime(year, 1, 1);
                    matches = _dystirDBContext.Matches.Where(y => y.MatchActivation != 1 && y.MatchActivation != 2 && y.Time > fromDate);
                    break;
            }
            var t = matches?.ToList();
            return matches;
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

            if (id != matches.MatchId)
            {
                return BadRequest();
            }
            try
            {
                Matches matchInDB = _dystirDBContext.Matches.Find(id);
                if (!(matches.ExtraMinutes == 0 && matches.ExtraSeconds == 0) || matchInDB.StatusId != matches.StatusId)
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
                IActionResult result = CreatedAtRoute("DefaultApi", new { id = matches.MatchId }, matches);
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
            return _dystirDBContext.Matches.Count(e => e.MatchId == id) > 0;
        }

        private void HubSend(Matches match)
        {
            HubSender hubSender = new HubSender();
            hubSender.SendMatch(_hubContext, match);
            HubSendMatchDetails(match);
        }

        private void HubSendMatchDetails(Matches match)
        {
            HubSender hubSender = new HubSender();
            MatchDetails matchDetails = new MatchDetailsController(_dystirDBContext).Get(match.MatchId);
            matchDetails.Match = match;
            hubSender.SendMatchDetails(_hubContext, matchDetails);
        }
    }
}