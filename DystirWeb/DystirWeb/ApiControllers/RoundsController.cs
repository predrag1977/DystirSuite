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
        private DystirDBContext _dystirDBContext;

        public RoundsController(DystirDBContext dystirDBContext)
        {
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Rounds
        [HttpGet]
        public IQueryable<Round> GetRounds()
        {
            return _dystirDBContext.Round;
        }

        // GET: api/Rounds/5
        [HttpGet("{id}", Name = "GetRounds")]
        public IActionResult GetRounds(int id)
        {
            Round round = _dystirDBContext.Round.Find(id);
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

            _dystirDBContext.Entry(round).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
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

            _dystirDBContext.Round.Add(round);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = round.Id }, round);
        }

        // DELETE: api/Rounds/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRounds(int id)
        {
            Round round = _dystirDBContext.Round.Find(id);
            if (round == null)
            {
                return NotFound();
            }

            _dystirDBContext.Round.Remove(round);
            _dystirDBContext.SaveChanges();

            return Ok(round);
        }

        private bool RoundsExists(int id)
        {
            return _dystirDBContext.Round.Count(e => e.Id == id) > 0;
        }
    }
}