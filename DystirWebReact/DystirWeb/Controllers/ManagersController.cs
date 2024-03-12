using DystirWeb.DystirDB;
using DystirWeb.Models;
using DystirWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DystirWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagersController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly DystirDBContext _dystirDBContext;

        public ManagersController(DystirDBContext dystirDBContext, AuthService authService)
        {
            _authService = authService;
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/Managers/token
        [HttpGet("{token}")]
        public IActionResult GetManagers(string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }
            return Ok(_dystirDBContext.Managers);
        }

        // GET: api/GetManager/5/token
        [HttpGet("{id}/{token}", Name = "GetManager")]
        public IActionResult GetManagers(int id, string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }
            Manager manager = _dystirDBContext.Managers.Find(id);
            if (manager == null)
            {
                return NotFound();
            }

            return Ok(manager);
        }

        // PUT: api/Manager/5
        [HttpPut("{id}/{token}")]
        public IActionResult PutManagers(int id, string token, [FromBody] Manager manager)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != manager.ManagerID)
            {
                return BadRequest();
            }
            try
            {
                Manager managerInDB = _dystirDBContext.Managers.Find(id);

                _dystirDBContext.Entry(managerInDB).CurrentValues.SetValues(manager);
                _dystirDBContext.Entry(managerInDB).State = EntityState.Modified;
                _dystirDBContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ManagerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw ex;
                }
            }
            return Ok();
        }

        // POST: api/Manager
        [HttpPost("{token}")]
        public IActionResult PostManagers(string token, [FromBody] Manager manager)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                bool existManager = _dystirDBContext.Managers.Any(x => x.DeviceToken == manager.DeviceToken);
                if(!existManager)
                {
                    _dystirDBContext.Managers.Add(manager);
                    _dystirDBContext.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // DELETE api/values/5/token
        [HttpDelete("{id}/{token}")]
        public void Delete(int id, string token)
        {
            Console.WriteLine($"{id} {token}");
        }

        private bool ManagerExists(int id)
        {
            return _dystirDBContext.Managers.Any(e => e.ID == id);
        }
    }
}

