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

    public async Task<BaseResponse<SleepDto>> GetSleepById(int id)
    {
        var responseWithDataDto = new BaseResponse<SleepDto>();

        responseWithDataDto.Status = ResponseStatus.Success;
        responseWithDataDto.Message = "Found";

        var returnedSleepDto = new SleepDto
        {
            Id = 1,
            Start = DateTime.Now.AddHours(-8).ToString("O"),
            End = DateTime.Now.ToString("O"),
            DurationHours = "8"
        };

        responseWithDataDto.Data = returnedSleepDto;

        return responseWithDataDto;
    }
}
