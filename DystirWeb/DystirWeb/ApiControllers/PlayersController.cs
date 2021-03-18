using System.Linq;
using System.Net;
using DystirWeb.Models;
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

        public PlayersController(DystirDBContext dystirDBContext)
        {
            _dystirDBContext = dystirDBContext;
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
        [HttpPut("{id}")]
        public IActionResult PutPlayers(int id, Players players)
        {
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
        [HttpPost]
        public IActionResult PostPlayers(Players players)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dystirDBContext.Players.Add(players);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = players.PlayerId }, players);
        }

        // DELETE: api/Players/5
        [HttpDelete("{id}")]
        public IActionResult DeletePlayers(int id)
        {
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
            return _dystirDBContext.Players.Count(e => e.PlayerId == id) > 0;
        }
    }
}