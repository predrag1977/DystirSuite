using System.Linq;
using System.Net;
using DystirWeb.DystirDB;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : ControllerBase
    {
        private DystirDBContext _dystirDBContext;

        public StatusesController(DystirDBContext dystirDBContext)
        {
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Statuses
        [HttpGet]
        public IQueryable<Statuses> GetStatuses()
        {
            return _dystirDBContext.Statuses;
        }

        // GET: api/Statuses/5
        [HttpGet("{id}", Name = "GetStatus")]
        public IActionResult GetStatuses(int id)
        {
            Statuses statuses = _dystirDBContext.Statuses.Find(id);
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

            _dystirDBContext.Entry(statuses).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
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

            _dystirDBContext.Statuses.Add(statuses);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = statuses.Id }, statuses);
        }

        // DELETE: api/Statuses/5
        [HttpDelete("{id}")]
        public IActionResult DeleteStatuses(int id)
        {
            Statuses statuses = _dystirDBContext.Statuses.Find(id);
            if (statuses == null)
            {
                return NotFound();
            }

            _dystirDBContext.Statuses.Remove(statuses);
            _dystirDBContext.SaveChanges();

            return Ok(statuses);
        }

        private bool StatusesExists(int id)
        {
            return _dystirDBContext.Statuses.Count(e => e.Id == id) > 0;
        }
    }
}