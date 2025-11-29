using SleepTracker.Api.Responses;
using SleepTracker.Api.Models;

namespace SleepTracker.Api.Repositories;

public interface ISleepRepository
{
    public Task<PagedResponse<List<Sleep>>> GetPagedSleeps(PaginationParams paginationParams);

    public Task<BaseResponse<Sleep>> GetSleepById(int id);

    public Task<BaseResponse<Sleep>> CreateSleep(Sleep sleep);
}
