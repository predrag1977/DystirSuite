using DystirWeb.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DystirWeb.Services;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticCompetitionsService _statisticCompetitionsService;

        public StatisticsController(StatisticCompetitionsService statisticCompetitionsService)
        {
            _statisticCompetitionsService = statisticCompetitionsService;
        }

        // GET api/statistic
        [HttpGet]
        public IEnumerable<CompetitionStatistic> Get()
        {
            return _statisticCompetitionsService.GetCompetitionsStatistic();
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