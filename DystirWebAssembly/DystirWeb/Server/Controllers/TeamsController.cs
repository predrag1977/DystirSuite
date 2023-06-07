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
    public class TeamsController : ControllerBase
    {
        private readonly DystirService _dystirService;
        private readonly DystirDBContext _dystirDBContext;

        public TeamsController(DystirService dystirService, DystirDBContext dystirDBContext)
        {
            _dystirService = dystirService;
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<IEnumerable<Teams>> GetTeams()
        {
            return await Task.FromResult(_dystirService.AllTeams);
        }

        // GET: api/Teams/5
        [HttpGet("{id}", Name = "GetTeam")]
        public IActionResult GetTeams(int id)
        {
            Teams teams = _dystirService.AllTeams.FirstOrDefault(x=>x.TeamId == id);
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

            _dystirDBContext.Entry(teams).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
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

            _dystirDBContext.Teams.Add(teams);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = teams.TeamId }, teams);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTeams(int id)
        {
            Teams teams = _dystirService.AllTeams.FirstOrDefault(x=>x.TeamId == id);
            if (teams == null)
            {
                return NotFound();
            }

            _dystirDBContext.Teams.Remove(teams);
            _dystirDBContext.SaveChanges();

            return Ok(teams);
        }

        private bool TeamsExists(int id)
        {
            return _dystirService.AllTeams.Any(e => e.TeamId == id);
        }
    }
}