using DystirWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchTypesController : ControllerBase
    {
        private DystirDBContext _dystirDBContext;

        public MatchTypesController(DystirDBContext dystirDBContext)
        {
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/MatchTypes
        [HttpGet]
        public IQueryable<MatchTypes> GetMatchTypes()
        {
            return _dystirDBContext.MatchTypes;
        }

        // GET: api/MatchTypes/5
        [HttpGet("{id}", Name = "GetMatchType")]
        public IActionResult GetMatchTypes(int id)
        {
            MatchTypes matchTypes = _dystirDBContext.MatchTypes.Find(id);
            if (matchTypes == null)
            {
                return NotFound();
            }

            return Ok(matchTypes);
        }

        // PUT: api/MatchTypes/5
        [HttpPut("{id}")]
        public IActionResult PutMatchTypes(int id, MatchTypes matchTypes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != matchTypes.Id)
            {
                return BadRequest();
            }

            _dystirDBContext.Entry(matchTypes).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchTypesExists(id))
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

        // POST: api/MatchTypes
        [HttpPost]
        public IActionResult PostMatchTypes(MatchTypes matchTypes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dystirDBContext.MatchTypes.Add(matchTypes);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = matchTypes.Id }, matchTypes);
        }

        // DELETE: api/MatchTypes/5
        [HttpDelete("{id}")]
        public IActionResult DeleteMatchTypes(int id)
        {
            MatchTypes matchTypes = _dystirDBContext.MatchTypes.Find(id);
            if (matchTypes == null)
            {
                return NotFound();
            }

            _dystirDBContext.MatchTypes.Remove(matchTypes);
            _dystirDBContext.SaveChanges();

            return Ok(matchTypes);
        }

        private bool MatchTypesExists(int id)
        {
            return _dystirDBContext.MatchTypes.Count(e => e.Id == id) > 0;
        }
    }
}