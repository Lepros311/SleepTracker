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

        try
        {
            var sleep = await _dbContext.Sleeps.FirstOrDefaultAsync(s => s.Id == id);

            if (sleep == null)
            {
                response.Status = ResponseStatus.Fail;
                response.Message = "Sleep record not found.";
            }
            else
            {
                response.Status = ResponseStatus.Success;
                response.Data = sleep;
            }
        }
        catch (Exception ex)
        {
            response.Message = $"Error in SleepRepository {nameof(SleepRepository)}: {ex.Message}";
            response.Status = ResponseStatus.Fail;
        }

        return response;
    }

    public async Task<BaseResponse<Sleep>> CreateSleep(Sleep newSleep)
    {
        var response = new BaseResponse<Sleep>();

        try
        {
            _dbContext.Sleeps.Add(newSleep);

            await _dbContext.SaveChangesAsync();

            if (newSleep == null)
            {
                response.Status = ResponseStatus.Fail;
                response.Message = "Sleep record not created.";
            }
            else
            {
                response.Status = ResponseStatus.Success;
                response.Data = newSleep;
            }
        }
        catch (Exception ex)
        {
            response.Message = $"Error in SleepRepository {nameof(CreateSleep)}: {ex.Message}";
            response.Status = ResponseStatus.Fail;
        }

        return response;
    }

    public async Task<BaseResponse<Sleep>> UpdateSleep(Sleep updatedSleep)
    {
        var response = new BaseResponse<Sleep>();

        try
        {
            var existingSleep = await _dbContext.Sleeps.FindAsync(updatedSleep.Id);
            if (existingSleep == null)
            {
                response.Status = ResponseStatus.Fail;
                response.Message = "No sleep record with that ID found.";
                return response;
            }

            _dbContext.Entry(existingSleep).CurrentValues.SetValues(updatedSleep);
            var affectedRows = await _dbContext.SaveChangesAsync();

            if (affectedRows == 0)
            {
                response.Status = ResponseStatus.Fail;
                response.Message = "No changes were saved.";
            }
            else
            {
                response.Status = ResponseStatus.Success;
                response.Data = updatedSleep;
            }
        }
        catch (Exception ex)
        {
            response.Message = $"Error in SleepRepository {nameof(UpdateSleep)}: {ex.Message}";
            response.Status = ResponseStatus.Fail;
        }

        return response;
    }

    public async Task<BaseResponse<Sleep>> DeleteSleep(int id)
    {
        var response = new BaseResponse<Sleep>();

        try
        {
            var existingSleep = await _dbContext.Sleeps.FindAsync(id);
            if (existingSleep == null)
            {
                response.Status = ResponseStatus.Fail;
                response.Message = "Sleep record not found.";
                return response;
            }

            existingSleep.IsDeleted = true;
            _dbContext.Sleeps.Update(existingSleep);
            var affectedRows = await _dbContext.SaveChangesAsync();

            if (affectedRows == 0)
            {
                response.Status = ResponseStatus.Fail;
                response.Message = "Deletion failed.";
            }
            else
            {
                response.Status = ResponseStatus.Success;
                response.Message = "Sleep record deleted.";
                response.Data = existingSleep;
            }
        }
        catch (Exception ex)
        {
            response.Message = $"Error in SleepRepository {nameof(DeleteSleep)}: {ex.Message}";
            response.Status = ResponseStatus.Fail;
        }

        return response;
    }
}
