using DystirWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.Services;
using DystirWeb.ModelViews;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        private readonly StandingService _standingService;
        private readonly DystirDBContext _dystirDBContext;

        public StandingsController(DystirDBContext dystirDBContext, StandingService standingService)
        {
            _standingService = standingService;
            _dystirDBContext = dystirDBContext;
        }

        // GET api/Standings
        [HttpGet]
        public IEnumerable<Standing> Get()
        {
            var teamsList = _dystirDBContext.Teams;
            DateTime date = new DateTime(DateTime.Now.Year, 1, 1);
            var matchesList = _dystirDBContext.Matches?.Where(x => x.MatchTypeID != null
                    && x.MatchActivation != 1
                    && x.MatchActivation != 2
                    && x.Time > date);
            return _standingService.GetStandings(teamsList, matchesList);
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