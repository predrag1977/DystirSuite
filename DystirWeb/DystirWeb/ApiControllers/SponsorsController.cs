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
    public class SponsorsController : ControllerBase
    {
        private DystirDBContext _dystirDBContext;

        public SponsorsController(DystirDBContext dystirDBContext)
        {
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Sponsors
        [HttpGet]
        public IQueryable<Sponsors> GetSponsors()
        {
            return _dystirDBContext.Sponsors;
        }

        // GET: api/Sponsors/5
        [HttpGet("{id}", Name = "GetSponsor")]
        public IActionResult GetSponsors(int id)
        {
            Sponsors sponsors = _dystirDBContext.Sponsors.Find(id);
            if (sponsors == null)
            {
                return NotFound();
            }

            return Ok(sponsors);
        }

        // PUT: api/Sponsors/5
        [HttpPut("{id}")]
        public IActionResult PutSponsors(int id, Sponsors sponsors)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sponsors.Id)
            {
                return BadRequest();
            }

            _dystirDBContext.Entry(sponsors).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SponsorsExists(id))
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

        // POST: api/Sponsors
        [HttpPost]
        public IActionResult PostSponsors(Sponsors sponsors)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dystirDBContext.Sponsors.Add(sponsors);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = sponsors.Id }, sponsors);
        }

        // DELETE: api/Sponsors/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSponsors(int id)
        {
            Sponsors sponsors = _dystirDBContext.Sponsors.Find(id);
            if (sponsors == null)
            {
                return NotFound();
            }

            _dystirDBContext.Sponsors.Remove(sponsors);
            _dystirDBContext.SaveChanges();

            return Ok(sponsors);
        }

        private bool SponsorsExists(int id)
        {
            return _dystirDBContext.Sponsors.Count(e => e.Id == id) > 0;
        }
    }
}