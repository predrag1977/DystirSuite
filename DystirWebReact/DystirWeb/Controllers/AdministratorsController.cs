using System.Linq;
using DystirWeb.Shared;
using DystirWeb.DystirDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DystirWeb.Services;
using System;

namespace DystirWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministratorsController : ControllerBase
    {
        private readonly AuthService _authService;
        private DystirDBContext _dystirDBContext;

        public AdministratorsController(DystirDBContext dystirDBContext, AuthService authService)
        {
            _authService = authService;
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Administrators/token
        [HttpGet("{token}")]
        public IActionResult GetAdministrators(string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }
            return Ok(_dystirDBContext.Administrators);
        }

        // GET: api/Administrators/5/token
        [HttpGet("{id}/{token}", Name = "GetAdministrator")]
        public IActionResult GetAdministrators(int id, string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }
            Administrators administrators = _dystirDBContext.Administrators.Find(id);
            if (administrators == null)
            {
                return NotFound();
            }

            return Ok(administrators);
        }

        // PUT: api/Administrators/5
        [HttpPut("{id}/{token}")]
        public IActionResult PutAdministrators(int id, string token, [FromBody] Administrators administrators)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

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
        [HttpPost("{token}")]
        public IActionResult PostAdministrators(string token, [FromBody] Administrators administrators)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dystirDBContext.Administrators.Add(administrators);
            _dystirDBContext.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = administrators.Id }, administrators);
        }

        // DELETE: api/Administrators/5
        [HttpDelete("{id}/{token}")]
        public IActionResult DeleteAdministrators(int id, string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

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