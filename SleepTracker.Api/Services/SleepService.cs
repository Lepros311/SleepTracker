using SleepTracker.Api.Models;
using SleepTracker.Api.Repositories;
using SleepTracker.Api.Responses;

namespace SleepTracker.Api.Services;

public class SleepService : ISleepService
{
    private readonly ISleepRepository _sleepRepository;

    public SleepService(ISleepRepository sleepRepository)
    {
        _sleepRepository = sleepRepository;
    }

    public async Task<PagedResponse<List<SleepDto>>> GetPagedSleeps(PaginationParams paginationParams)
    {
        var response = await _sleepRepository.GetPagedSleeps(paginationParams);

        var responseWithDataDto = new PagedResponse<List<SleepDto>>(data: new List<SleepDto>(),
                                                                    pageNumber: paginationParams.Page,
                                                                    pageSize: paginationParams.PageSize,
                                                                    totalRecords: response.TotalRecords);

        if (response.Status == ResponseStatus.Fail)
        {
            responseWithDataDto.Status = response.Status;
            responseWithDataDto.Message = response.Message;
            return responseWithDataDto;
        }

        responseWithDataDto.Data = response.Data.Select(s => new SleepDto
        {
            Id = s.Id
        }).ToList();

        return responseWithDataDto;
    }
}
