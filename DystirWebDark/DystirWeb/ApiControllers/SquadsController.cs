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
    public class SquadsController : ControllerBase
    {
        private DystirDBContext db;

        public SquadsController(DystirDBContext dystirDBContext)
        {
            db = dystirDBContext;
        }

        // GET: api/Squads
        [HttpGet]
        public IQueryable<Squad> GetSquad()
        {
            return db.Squad;
        }

        // GET: api/Squads/5
        [HttpGet("{id}", Name = "GetSquad")]
        public IActionResult GetSquad(int id)
        {
            Squad squad = db.Squad.Find(id);
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

            db.Entry(squad).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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

            db.Squad.Add(squad);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = squad.Id }, squad);
        }

        // DELETE: api/Squads/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSquad(int id)
        {
            Squad squad = db.Squad.Find(id);
            if (squad == null)
            {
                return NotFound();
            }

            db.Squad.Remove(squad);
            db.SaveChanges();

            return Ok(squad);
        }

        private bool SquadExists(int id)
        {
            return db.Squad.Count(e => e.Id == id) > 0;
        }
    }
}