using DystirWeb.Controllers;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace DystirWeb.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchDetailsController : ControllerBase
    {
        private DystirDBContext dbContext;

        public MatchDetailsController(DystirDBContext dystirDBContext)
        {
            dbContext = dystirDBContext;
        }

        // GET api/<controller>/5
        [HttpGet("{id}", Name = "GetMatchDetails")]
        public MatchDetails Get(int id)
        {
            Matches match = (new MatchesController(null, dbContext).GetMatches(id) as OkObjectResult).Value as Matches;
            var eventsOfMatch = new EventsOfMatchesController(null, dbContext).GetEventsOfMatchesByMatchId(id);
            var playersOfMatch = new PlayersOfMatchesController(null, dbContext).GetPlayersOfMatchesByMatchID(id);
            MatchDetails matchDetails = new MatchDetails()
            {
                Match = match,
                EventsOfMatch = eventsOfMatch?
                .OrderBy(x => x.EventPeriodId ?? 0)
                .ThenBy(x => x.EventTotalTime)
                .ThenBy(x => x.EventMinute)
                .ThenBy(x => x.EventOfMatchId).ToList(),
                PlayersOfMatch = playersOfMatch.Where(x => x.PlayingStatus != 3).ToList()
            };
            return matchDetails;
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