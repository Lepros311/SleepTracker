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
        var response = await _sleepRepository.GetSleepById(id);

        if (response.Status == ResponseStatus.Fail || response.Data == null)
        {
            return new BaseResponse<SleepDto>
            {
                Status = ResponseStatus.Fail,
                Message = response.Message,
                Data = null
            };
        }

        var sleep = response.Data;

        var sleepDto = new SleepDto
        {
            Id = sleep.Id,
            Start = sleep.Start.ToString("O"),
            End = sleep.End.ToString("O"),
            DurationHours = (sleep.End - sleep.Start).TotalHours.ToString()
        };

        return new BaseResponse<SleepDto>
        {
            Status = ResponseStatus.Success,
            Message = response.Message,
            Data = sleepDto
        };
    }
}
