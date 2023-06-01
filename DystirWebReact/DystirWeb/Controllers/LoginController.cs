using DystirWeb.Services;
using Microsoft.AspNetCore.Mvc;
using DystirWeb.Shared;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly DystirService _dystirService;
        private readonly AuthService _authService;

        public LoginController(DystirService dystirService, AuthService authService)
        {
            _dystirService = dystirService;
            _authService = authService;
        }

        // GET: api/Login/token
        [HttpGet("{token}", Name = "GetLogin")]
        public IActionResult Get(string token)
        {
            if (_authService.IsAuthorized(token ?? ""))
            {
                Administrators administartor = _dystirService.DystirDBContext.Administrators.FirstOrDefault(x => x.AdministratorToken == token);
                return Ok(administartor);
            }
            else
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }
        }

        // POST api/values
        [HttpPost("{id}")]
        public void Post(int id, [FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
