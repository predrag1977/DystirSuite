using System;
using System.Linq;
using System.Net;
using DystirWeb.Server.DystirDB;
using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private DystirDBContext _dystirDBContext;
        private readonly AuthService _authService;

        public PlayersController(DystirDBContext dystirDBContext, AuthService authService)
        {
            _dystirDBContext = dystirDBContext;
            _authService = authService;
        }

        // GET: api/Players
        [HttpGet]
        public IQueryable<Players> GetPlayers()
        {
            return _dystirDBContext.Players;
        }

        // GET: api/Players?teamID=1
        public IQueryable<Players> GetPlayersOfMatchesByMatchID(int teamID)
        {
            return _dystirDBContext.Players.Where(x => x.TeamId == teamID);
        }

        // GET: api/Players/5
        [HttpGet("{id}", Name = "GetPlayer")]
        public IActionResult GetPlayers(int id)
        {
            Players players = _dystirDBContext.Players.Find(id);
            if (players == null)
            {
                return NotFound();
            }

            return Ok(players);
        }

        // PUT: api/Players/5
        [HttpPut("{id}/{token}")]
        public IActionResult PutPlayers(int id, string token, [FromBody] Players players)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != players.PlayerId)
            {
                return BadRequest();
            }

            _dystirDBContext.Entry(players).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(StatusCodes.Status204NoContent);
        }

        // POST: api/Players
        [HttpPost("{token}")]
        public IActionResult PostPlayers(string token, [FromBody] Players players)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dystirDBContext.Players.Add(players);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = players.PlayerId }, players);
        }

        // DELETE: api/Players/5
        [HttpDelete("{id}/{token}")]
        public IActionResult DeletePlayers(int id, string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            Players players = _dystirDBContext.Players.Find(id);
            if (players == null)
            {
                return NotFound();
            }

            _dystirDBContext.Players.Remove(players);
            _dystirDBContext.SaveChanges();

            return Ok(players);
        }

        private bool PlayersExists(int id)
        {
            return _dystirDBContext.Players.Any(e => e.PlayerId == id);
        }
    }
}