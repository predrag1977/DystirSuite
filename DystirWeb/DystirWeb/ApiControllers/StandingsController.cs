using DystirWeb.Models;
using DystirWeb.ModelViews;
using DystirWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DystirWeb.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        // GET api/Standings
        [HttpGet]
        public IEnumerable<Standing> Get()
        {
            var teamsList = DystirService.AllTeams;
            DateTime date = new DateTime(DateTime.Now.Year, 1, 1);
            var matchesList = DystirService.AllMatches?.Where(x => x.MatchTypeId != null
                    && x.MatchActivation != 1
                    && x.MatchActivation != 2
                    && x.Time > date);
            return DystirService.StandingService.GetStandings(teamsList, matchesList);
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