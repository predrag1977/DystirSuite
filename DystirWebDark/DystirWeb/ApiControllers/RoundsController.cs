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
    public class RoundsController : ControllerBase
    {
        private DystirDBContext db;

        public RoundsController(DystirDBContext dystirDBContext)
        {
            db = dystirDBContext;
        }

        // GET: api/Rounds
        [HttpGet]
        public IQueryable<Round> GetRounds()
        {
            return db.Round;
        }

        // GET: api/Rounds/5
        [HttpGet("{id}", Name = "GetRounds")]
        public IActionResult GetRounds(int id)
        {
            Round round = db.Round.Find(id);
            if (round == null)
            {
                return NotFound();
            }

            return Ok(round);
        }

        // PUT: api/Rounds/5
        [HttpPut("{id}")]
        public IActionResult PutRounds(int id, Round round)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != round.Id)
            {
                return BadRequest();
            }

            db.Entry(round).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoundsExists(id))
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

        // POST: api/Rounds
        [HttpPost]
        public IActionResult PostRounds(Round round)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Round.Add(round);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = round.Id }, round);
        }

        // DELETE: api/Rounds/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRounds(int id)
        {
            Round round = db.Round.Find(id);
            if (round == null)
            {
                return NotFound();
            }

            db.Round.Remove(round);
            db.SaveChanges();

            return Ok(round);
        }

        private bool RoundsExists(int id)
        {
            return db.Round.Count(e => e.Id == id) > 0;
        }
    }
}