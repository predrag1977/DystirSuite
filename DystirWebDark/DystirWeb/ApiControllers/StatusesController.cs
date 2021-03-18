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
    public class StatusesController : ControllerBase
    {
        private DystirDBContext db;

        public StatusesController(DystirDBContext dystirDBContext)
        {
            db = dystirDBContext;
        }

        // GET: api/Statuses
        [HttpGet]
        public IQueryable<Statuses> GetStatuses()
        {
            return db.Statuses;
        }

        // GET: api/Statuses/5
        [HttpGet("{id}", Name = "GetStatus")]
        public IActionResult GetStatuses(int id)
        {
            Statuses statuses = db.Statuses.Find(id);
            if (statuses == null)
            {
                return NotFound();
            }

            return Ok(statuses);
        }

        // PUT: api/Statuses/5
        [HttpPut("{id}")]
        public IActionResult PutStatuses(int id, Statuses statuses)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != statuses.Id)
            {
                return BadRequest();
            }

            db.Entry(statuses).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatusesExists(id))
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

        // POST: api/Statuses
        [HttpPost]
        public IActionResult PostStatuses(Statuses statuses)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Statuses.Add(statuses);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = statuses.Id }, statuses);
        }

        // DELETE: api/Statuses/5
        [HttpDelete("{id}")]
        public IActionResult DeleteStatuses(int id)
        {
            Statuses statuses = db.Statuses.Find(id);
            if (statuses == null)
            {
                return NotFound();
            }

            db.Statuses.Remove(statuses);
            db.SaveChanges();

            return Ok(statuses);
        }

        private bool StatusesExists(int id)
        {
            return db.Statuses.Count(e => e.Id == id) > 0;
        }
    }
}