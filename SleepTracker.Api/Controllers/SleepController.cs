using Microsoft.AspNetCore.Mvc;
using SleepTracker.Api.Models;
using SleepTracker.Api.Responses;
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
        public async Task<ActionResult<PagedResponse<List<SleepDto>>>> GetPagedSleeps([FromQuery] PaginationParams paginationParams)
        {
            var responseWithDtos = await _sleepService.GetPagedSleeps(paginationParams);

            if (responseWithDtos.Status == ResponseStatus.Fail)
            {
                return BadRequest(responseWithDtos.Message);
            }

            return Ok(responseWithDtos);
        }
    }
}