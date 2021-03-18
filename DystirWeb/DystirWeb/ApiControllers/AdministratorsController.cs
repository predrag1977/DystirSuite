using System.Linq;
using DystirWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DystirWeb.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController : ControllerBase
    {
        private DystirDBContext _dystirDBContext;

        public AdministratorsController(DystirDBContext dystirDBContext)
        {
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Administrators
        [HttpGet]
        public IQueryable<Administrators> GetAdministrators()
        {
            return _dystirDBContext.Administrators;
        }

        // GET: api/Administrators/5
        [HttpGet("{id}", Name = "GetAdministrator")]
        public IActionResult GetAdministrators(int id)
        {
            Administrators administrators = _dystirDBContext.Administrators.Find(id);
            if (administrators == null)
            {
                return NotFound();
            }

            return Ok(administrators);
        }

        // PUT: api/Administrators/5
        [HttpPut("{id}")]
        public IActionResult PutAdministrators(int id, [FromBody] Administrators administrators)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != administrators.Id)
            {
                return BadRequest();
            }

            _dystirDBContext.Entry(administrators).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdministratorsExists(id))
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

        // POST: api/Administrators
        [HttpPost]
        public IActionResult PostAdministrators(Administrators administrators)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dystirDBContext.Administrators.Add(administrators);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = administrators.Id }, administrators);
        }

        // DELETE: api/Administrators/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAdministrators(int id)
        {
            Administrators administrators = _dystirDBContext.Administrators.Find(id);
            if (administrators == null)
            {
                return NotFound();
            }

            _dystirDBContext.Administrators.Remove(administrators);
            _dystirDBContext.SaveChanges();

            return Ok(administrators);
        }

        private bool AdministratorsExists(int id)
        {
            return _dystirDBContext.Administrators.Count(e => e.Id == id) > 0;
        }
    }
}