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
    public class TeamsController : ControllerBase
    {
        private DystirDBContext db;
        public TeamsController(DystirDBContext dystirDBContext)
        {
            db = dystirDBContext;
        }

        // GET: api/Teams
        [HttpGet]
        public IQueryable<Teams> GetTeams()
        {
            return db.Teams;
        }

        // GET: api/Teams/5
        [HttpGet("{id}", Name = "GetTeam")]
        public IActionResult GetTeams(int id)
        {
            Teams teams = db.Teams.Find(id);
            if (teams == null)
            {
                return NotFound();
            }

            return Ok(teams);
        }

        // PUT: api/Teams/5
        [HttpPut("{id}")]
        public IActionResult PutTeams(int id, Teams teams)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != teams.TeamId)
            {
                return BadRequest();
            }

            db.Entry(teams).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamsExists(id))
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

        // POST: api/Teams
        [HttpPost]
        public IActionResult PostTeams(Teams teams)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Teams.Add(teams);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = teams.TeamId }, teams);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTeams(int id)
        {
            Teams teams = db.Teams.Find(id);
            if (teams == null)
            {
                return NotFound();
            }

            db.Teams.Remove(teams);
            db.SaveChanges();

            return Ok(teams);
        }

        private bool TeamsExists(int id)
        {
            return db.Teams.Count(e => e.TeamId == id) > 0;
        }
    }
}