using System.Threading.Tasks;
using DystirWeb.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using DystirWeb.Hubs;
using DystirWeb.Shared;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshController : ControllerBase
    {
        private readonly IHubContext<DystirHub> _hubContext;
        private readonly DystirService _dystirService;

        public RefreshController(IHubContext<DystirHub> hubContext, DystirService dystirService)
        {
            _hubContext = hubContext;
            _dystirService = dystirService;
        }

        // GET: api/Refresh
        [HttpGet]
        public async Task<bool> Get()
        {
            try
            {
                await _dystirService.Refresh();
                HubSend();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // GET: api/Test/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Test
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Test/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        
        private void HubSend()
        {
            HubSender hubSender = new HubSender();
            hubSender.SendRefreshData(_hubContext);
        }
    }
}
