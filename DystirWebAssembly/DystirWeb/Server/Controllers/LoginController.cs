using DystirWeb.Services;
using Microsoft.AspNetCore.Mvc;
using DystirWeb.Shared;
using System.Linq;
using System;
using System.Threading.Tasks;
using DystirWeb.Server.DystirDB;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly DystirDBContext _dystirDBContext;
        private readonly AuthService _authService;

        public LoginController(DystirDBContext dystirDBContext, AuthService authService)
        {
            _dystirDBContext = dystirDBContext;
            _authService = authService;
        }

        // GET: api/Login/token
        [HttpGet("{token}", Name = "GetLogin")]
        public IActionResult Get(string token)
        {
            if (_authService.IsAuthorized(token ?? ""))
            {
                Administrators administartor = _dystirDBContext.Administrators.FirstOrDefault(x => x.AdministratorToken == token);
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
