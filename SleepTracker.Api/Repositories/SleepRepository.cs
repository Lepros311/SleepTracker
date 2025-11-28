using Microsoft.EntityFrameworkCore;
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

        try
        {
            var query = _dbContext.Sleeps.AsQueryable();

            if (paginationParams.Start != null)
                query = query.Where(s => s.Start.Date == paginationParams.Start.Value.Date);
            if (paginationParams.End != null)
                query = query.Where(s => s.End.Date == paginationParams.End.Value.Date);
            if (paginationParams.MinDurationHours.HasValue)
                query = query.Where(s => s.DurationHours >= paginationParams.MinDurationHours.Value);
            if (paginationParams.MaxDurationHours.HasValue)
                query = query.Where(s => s.DurationHours <= paginationParams.MaxDurationHours.Value);

            var totalRecords = await query.CountAsync();

            var sortBy = paginationParams.SortBy?.Trim().ToLower() ?? "id";
            var sortAscending = paginationParams.SortAscending;

            bool useAscending = sortAscending ?? (sortBy == "id" ? false : true);

            query = sortBy switch
            {
                "start" => useAscending ? query.OrderBy(s => s.Start.Date) : query.OrderByDescending(s => s.Start.Date),
                "end" => useAscending ? query.OrderBy(s => s.End.Date) : query.OrderByDescending(s => s.End.Date),
                "durationHours" => useAscending ? query.OrderBy(s => s.DurationHours) : query.OrderByDescending(s => s.DurationHours),
                _ => useAscending ? query.OrderBy(s => s.Id) : query.OrderByDescending(s => s.Id)
            };

            var pagedSleeps = await query.Skip((paginationParams.Page - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            response.Status = ResponseStatus.Success;
            response.Data = pagedSleeps;
            response.TotalRecords = totalRecords;
        }
        catch (Exception ex)
        {
            response.Message = $"Error in SleepRepository {nameof(GetPagedSleeps)}: {ex.Message}";
            response.Status = ResponseStatus.Fail;
        }

        return response;
    }

    public async Task<BaseResponse<Sleep>> GetSleepById(int id)
    {
        var response = new BaseResponse<Sleep>();

        return response;
    }
}
