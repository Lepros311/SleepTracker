using SleepTracker.Api.Data;
using SleepTracker.Api.Models;
using SleepTracker.Api.Responses;

namespace SleepTracker.Api.Repositories;

public class SleepRepository : ISleepRepository
{
    private readonly SleepTrackerDbContext _dbContext;

    public SleepRepository(SleepTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<List<Sleep>>> GetPagedSleeps(PaginationParams paginationParams)
    {
        var response = new PagedResponse<List<Sleep>>(data: new List<Sleep>(),
                                                      pageNumber: paginationParams.Page, 
                                                      pageSize: paginationParams.PageSize,
                                                      totalRecords: 0);

        return response;
    }
}
