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
        private readonly DystirService _dystirService;

        public ManagersController(DystirService dystirService, AuthService authService)
        {
            _authService = authService;
            _dystirService = dystirService;
        }

        // GET: api/Managers/token
        [HttpGet("{token}")]
        public IActionResult GetManagers(string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }
            return Ok(_dystirService.AllManagers);
        }

        // GET: api/GetManager/5/token
        [HttpGet("{id}/{token}", Name = "GetManager")]
        public IActionResult GetManagers(int id, string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }
            Manager manager = _dystirService.AllManagers.FirstOrDefault(x => x.ID == id);
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
                Manager managerInDB = _dystirService.AllManagers.FirstOrDefault(x=>x.ID == id);

                _dystirService.DystirDBContext.Entry(managerInDB).CurrentValues.SetValues(manager);
                _dystirService.DystirDBContext.Entry(managerInDB).State = EntityState.Modified;
                _dystirService.DystirDBContext.SaveChanges();
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
                bool existManager = _dystirService.AllManagers.Any(x => x.DeviceToken == manager.DeviceToken);
                if(!existManager)
                {
                    var managerSameID = _dystirService.AllManagers.FirstOrDefault(x => x.ManagerID == manager.ManagerID);
                    if(managerSameID != null)
                    {
                        manager.MatchID = managerSameID.MatchID;
                    }
                    _dystirService.DystirDBContext.Managers.Add(manager);
                    _dystirService.DystirDBContext.SaveChanges();
                    _dystirService.AllManagers.Add(manager);
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
            return _dystirService.AllManagers.Any(e => e.ID == id);
        }
    }
}

