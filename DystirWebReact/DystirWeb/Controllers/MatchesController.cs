using System.Diagnostics;
using DystirWeb.DystirDB;
using DystirWeb.Hubs;
using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DystirWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IHubContext<DystirHub> _hubContext;
        private readonly AuthService _authService;
        private readonly DystirService _dystirService;
        private readonly MatchDetailsService _matchDetailsService;
        private readonly PushNotificationService _pushNotificationService;
        private DystirDBContext _dystirDBContext;

        public MatchesController (IHubContext<DystirHub> hubContext,
            AuthService authService,
            DystirDBContext dystirDBContext,
            DystirService dystirService,
            MatchDetailsService matchDetailsService,
            PushNotificationService pushNotificationService)
        {
            _hubContext = hubContext;
            _authService = authService;
            _dystirDBContext = dystirDBContext;
            _dystirService = dystirService;
            _matchDetailsService = matchDetailsService;
            _pushNotificationService = pushNotificationService;
        }

        // GET: api/matches
        [HttpGet("{parameter?}")]
        public IActionResult GetMatches(string parameter)
        {
            Debug.WriteLine("Start:" + DateTime.Now.ToString("hh:mm:ss:ff"));
            IEnumerable<Matches> matches;
            int year = DateTime.UtcNow.Year;
            var fromDate = DateTime.UtcNow.Date.AddDays(-15);
            var toDate = DateTime.UtcNow.Date.AddDays(15);
            switch (parameter?.ToLower())
            {
                case "matches":
                    matches = _dystirService.AllMatches.Where(x => x.Time > fromDate && x.Time < toDate && x.MatchActivation != 1 && x.MatchActivation != 2);
                    break;
                case "results":
                    matches = _dystirService.AllMatches.Where(y => y.MatchActivation != 1 && y.MatchActivation != 2);
                    break;
                case "fixtures":
                    matches = _dystirService.AllMatches.Where(y => y.MatchActivation != 1 && y.MatchActivation != 2);
                    break;
                case "archived":
                    matches = _dystirService.AllMatches.Where(y => y.MatchActivation == 1);
                    break;
                default:
                    fromDate = new DateTime(year, 1, 1);
                    matches = _dystirService.AllMatches.Where(y => y.Time > fromDate
                            && y.MatchActivation != 1
                            && y.MatchActivation != 2);
                    break;
            }
            Debug.WriteLine("Finished:" + DateTime.Now.ToString("hh:mm:ss:ff"));
            return Ok(matches);
        }

        // PUT: api/Matches/5
        [HttpPut("{id}/{token}")]
        public IActionResult PutMatches(int id, string token, [FromBody] Matches match)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != match.MatchID)
            {
                return BadRequest();
            }
            try
            {
                Matches matchInDB = _dystirDBContext.Matches.Find(id);
                var isMatchTimeCorrection = match.ExtraMinutes > 0 || match.ExtraSeconds > 0;
                var isMatchStatusChanged = matchInDB.StatusID != match.StatusID;
                if (!(match.ExtraMinutes == 0 && match.ExtraSeconds == 0) || matchInDB.StatusID != match.StatusID)
                {
                    match.StatusTime = DateTime.UtcNow.AddMinutes(-match.ExtraMinutes).AddSeconds(-match.ExtraSeconds);
                }
                match.HomeTeamPenaltiesScore = matchInDB.HomeTeamPenaltiesScore;
                match.AwayTeamPenaltiesScore = matchInDB.AwayTeamPenaltiesScore;
                
                _dystirDBContext.Entry(matchInDB).CurrentValues.SetValues(match);
                _dystirDBContext.Entry(matchInDB).State = EntityState.Modified;
                _dystirDBContext.SaveChanges();
                HubSend(match);
                if (isMatchTimeCorrection || isMatchStatusChanged)
                {
                    _pushNotificationService.SendNotificationFromMatchAsync(match, isMatchTimeCorrection, isMatchStatusChanged);
                }  
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
            return Ok(match);
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
            return _dystirDBContext.Matches.Count(e => e.MatchID == id) > 0;
        }

        private void HubSend(Matches match)
        {
            HubSender hubSender = new HubSender();
            hubSender.SendMatch(_hubContext, match);
            HubSendMatchDetails(hubSender, match);
        }

        private async void HubSendMatchDetails(HubSender hubSender, Matches match)
        {
            await _dystirService.UpdateAllMatchesAsync(match);
            MatchDetails matchDetails = _matchDetailsService.GetMatchDetails(match.MatchID, true);
            await _dystirService.UpdateDataAsync(matchDetails);
            hubSender.SendMatchDetails(_hubContext, matchDetails);
        }
    }
}