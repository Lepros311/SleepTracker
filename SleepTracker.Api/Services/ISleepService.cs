using SleepTracker.Api.Models;
using SleepTracker.Api.Responses;

namespace SleepTracker.Api.Services;

public interface ISleepService
{
    Task<PagedResponse<List<SleepReadDto>>> GetPagedSleeps(PaginationParams paginationParams);

    Task<BaseResponse<SleepReadDto>> GetSleepById(int id);

    Task<BaseResponse<SleepReadDto>> CreateSleep(SleepCreateDto sleepCreateDto);
}
