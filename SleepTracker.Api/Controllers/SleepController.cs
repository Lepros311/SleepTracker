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
        public async Task<ActionResult<PagedResponse<List<SleepReadDto>>>> GetPagedSleeps([FromQuery] PaginationParams paginationParams)
        {
            var responseWithDtos = await _sleepService.GetPagedSleeps(paginationParams);

            if (responseWithDtos.Status == ResponseStatus.Fail)
            {
                return BadRequest(responseWithDtos.Message);
            }

            return Ok(responseWithDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SleepReadDto>> GetSleepById(int id)
        {
            var response = await _sleepService.GetSleepById(id);

            if (response.Status == ResponseStatus.Fail)
            {
                return NotFound(response.Message);
            }

            var returnedSleepDto = response.Data;

            return Ok(returnedSleepDto);
        }

        [HttpPost]
        public async Task<ActionResult<SleepReadDto>> CreateSleep([FromBody] SleepCreateDto sleepCreateDto)
        {
            var responseWithDataDto = await _sleepService.CreateSleep(sleepCreateDto);

            if (responseWithDataDto.Status == ResponseStatus.Fail)
            {
                return BadRequest(responseWithDataDto.Message);
            }

            return CreatedAtAction(nameof(GetSleepById), new { id = responseWithDataDto.Data.Id }, responseWithDataDto.Data);
        }

        [HttpPut]
        public async Task<ActionResult<SleepReadDto>> UpdateSleep(int id, [FromBody] SleepUpdateDto sleepUpdateDto)
        {
            var response = await _sleepService.UpdateSleep(id, sleepUpdateDto);

            if (response.Status == ResponseStatus.Fail)
            {
                return BadRequest(response.Message);
            }

            return NoContent();
        }
    }
}