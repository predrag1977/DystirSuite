using DystirWeb.Shared;
using DystirWeb.Server.DystirDB;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DystirWeb.Services;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        private readonly StandingService _standingService;

        public StandingsController(DystirDBContext dystirDBContext, StandingService standingService)
        {
            _standingService = standingService;
        }

        // GET api/Standings
        [HttpGet]
        public IEnumerable<Standing> Get()
        {
            return _standingService.GetStandings();
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}