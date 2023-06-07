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
    public class SquadsController : ControllerBase
    {
        private DystirDBContext _dystirDBContext;

        public SquadsController(DystirDBContext dystirDBContext)
        {
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Squads
        [HttpGet]
        public IQueryable<Squad> GetSquad()
        {
            return _dystirDBContext.Squad;
        }

        // GET: api/Squads/5
        [HttpGet("{id}", Name = "GetSquad")]
        public IActionResult GetSquad(int id)
        {
            Squad squad = _dystirDBContext.Squad.Find(id);
            if (squad == null)
            {
                return NotFound();
            }

            return Ok(squad);
        }

        // PUT: api/Squads/5
        [HttpPut("{id}")]
        public IActionResult PutSquad(int id, Squad squad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != squad.Id)
            {
                return BadRequest();
            }

            _dystirDBContext.Entry(squad).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SquadExists(id))
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

        // POST: api/Squads
        [HttpPost]
        public IActionResult PostSquad(Squad squad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dystirDBContext.Squad.Add(squad);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = squad.Id }, squad);
        }

        // DELETE: api/Squads/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSquad(int id)
        {
            Squad squad = _dystirDBContext.Squad.Find(id);
            if (squad == null)
            {
                return NotFound();
            }

            _dystirDBContext.Squad.Remove(squad);
            _dystirDBContext.SaveChanges();

            return Ok(squad);
        }

        private bool SquadExists(int id)
        {
            return _dystirDBContext.Squad.Any(e => e.Id == id);
        }
    }
}