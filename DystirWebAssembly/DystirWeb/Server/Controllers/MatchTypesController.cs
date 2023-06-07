using DystirWeb.Server.DystirDB;
using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchTypesController : ControllerBase
    {
        private readonly DystirService _dystirService;
        private readonly DystirDBContext _dystirDBContext;

        public MatchTypesController(DystirService dystirService ,DystirDBContext dystirDBContext)
        {
            _dystirService = dystirService;
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/MatchTypes
        [HttpGet]
        public async Task<IEnumerable<MatchTypes>> GetMatchTypes()
        {
            return await Task.FromResult(_dystirService.AllCompetitions);
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
            return _dystirDBContext.MatchTypes.Any(e => e.Id == id);
        }
    }
}