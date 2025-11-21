using SleepTracker.Api.Models;
using SleepTracker.Api.Responses;

namespace SleepTracker.Api.Services;

public interface ISleepService
{
    Task<PagedResponse<List<SleepDto>>> GetPagedSleeps(PaginationParams paginationParams);
}
