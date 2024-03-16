using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchDetailsController : ControllerBase
    {
        private readonly MatchDetailsService _matchDetailsService;

        public MatchDetailsController(MatchDetailsService matchDetailsService)
        {
            _matchDetailsService = matchDetailsService;
        }

        // GET api/matchDetails/5
        [HttpGet("{id}", Name = "GetMatchDetails")]
        public MatchDetails Get(int id)
        {
            return _matchDetailsService.FindMatchDetails(id); 
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