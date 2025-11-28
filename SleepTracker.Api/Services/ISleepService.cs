using SleepTracker.Api.Models;
using SleepTracker.Api.Responses;

namespace SleepTracker.Api.Services;

public interface ISleepService
{
    Task<PagedResponse<List<SleepDto>>> GetPagedSleeps(PaginationParams paginationParams);

    Task<BaseResponse<SleepDto>> GetSleepById(int id);

    Task<BaseResponse<SleepDto>> CreateSleep(SleepDto sleepDto);
}
