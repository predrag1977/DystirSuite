using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SponsorsController : ControllerBase
    {
        private readonly DystirService _dystirService;

        public SponsorsController(DystirService dystirService)
        {
            _dystirService = dystirService;
        }

        // GET: api/Sponsors
        [HttpGet]
        public async Task<IEnumerable<Sponsors>> GetSponsors()
        {
            return await Task.FromResult(_dystirService.AllSponsors);
        }

        // GET: api/Sponsors/5
        [HttpGet("{id}", Name = "GetSponsor")]
        public IActionResult GetSponsors(int id)
        {
            Sponsors sponsors = _dystirService.DystirDBContext.Sponsors.Find(id);
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

            _dystirService.DystirDBContext.Entry(sponsors).State = EntityState.Modified;

            try
            {
                _dystirService.DystirDBContext.SaveChanges();
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

            _dystirService.DystirDBContext.Sponsors.Add(sponsors);
            _dystirService.DystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = sponsors.Id }, sponsors);
        }

        // DELETE: api/Sponsors/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSponsors(int id)
        {
            Sponsors sponsors = _dystirService.DystirDBContext.Sponsors.Find(id);
            if (sponsors == null)
            {
                return NotFound();
            }

            _dystirService.DystirDBContext.Sponsors.Remove(sponsors);
            _dystirService.DystirDBContext.SaveChanges();

            return Ok(sponsors);
        }

        private bool SponsorsExists(int id)
        {
            return _dystirService.DystirDBContext.Sponsors.Count(e => e.Id == id) > 0;
        }
    }
}