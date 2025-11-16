using Microsoft.AspNetCore.Mvc;
using SleepTracker.Api.Services;

namespace SleepTracker.Api.Controllers
{
    [Route("api/sleeps")]
    [ApiController]
    public class SleepController : ControllerBase
    {
        private readonly ISleepService _sleepService;

        public SleepController(ISleepService sleepService)
        {
            _sleepService = sleepService;
        }

        [HttpGet]
        public async Task<ActionResult>
    }
}