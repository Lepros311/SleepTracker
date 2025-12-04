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

    public async Task<PagedResponse<List<SleepReadDto>>> GetPagedSleeps(PaginationParams paginationParams)
    {
        var response = await _sleepRepository.GetPagedSleeps(paginationParams);

        var responseWithDataDto = new PagedResponse<List<SleepReadDto>>(data: new List<SleepReadDto>(),
                                                                    pageNumber: paginationParams.Page,
                                                                    pageSize: paginationParams.PageSize,
                                                                    totalRecords: response.TotalRecords);

        if (response.Status == ResponseStatus.Fail)
        {
            responseWithDataDto.Status = response.Status;
            responseWithDataDto.Message = response.Message;
            return responseWithDataDto;
        }

        responseWithDataDto.Data = response.Data.Select(s => new SleepReadDto
        {
            Id = s.Id
        }).ToList();

        return responseWithDataDto;
    }

    public async Task<BaseResponse<SleepReadDto>> GetSleepById(int id)
    {
        var response = await _sleepRepository.GetSleepById(id);

        if (response.Status == ResponseStatus.Fail || response.Data == null)
        {
            return new BaseResponse<SleepReadDto>
            {
                Status = ResponseStatus.Fail,
                Message = response.Message,
                Data = null
            };
        }

        var sleep = response.Data;

        var sleepDto = new SleepReadDto
        {
            Id = sleep.Id,
            Start = sleep.Start.ToString("O"),
            End = sleep.End.ToString("O"),
            DurationHours = (sleep.End - sleep.Start).TotalHours.ToString()
        };

        return new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Success,
            Message = response.Message,
            Data = sleepDto
        };
    }

    public async Task<BaseResponse<SleepReadDto>> CreateSleep(SleepCreateDto sleepCreateDto)
    {
        var responseWithDataDto = new BaseResponse<SleepReadDto>();

        var start = DateTime.Parse(sleepCreateDto.Start);
        var end = DateTime.Parse(sleepCreateDto.End);

        if (start >= end)
        {
            responseWithDataDto.Status = ResponseStatus.Fail;
            responseWithDataDto.Message = "Start time must be earlier than end time.";
            return responseWithDataDto;
        }

        var newSleep = new Sleep
        {
            Start = DateTime.Parse(sleepCreateDto.Start),
            End = DateTime.Parse(sleepCreateDto.End)
        };

        var response = await _sleepRepository.CreateSleep(newSleep);

        if (response.Status == ResponseStatus.Fail)
        {
            responseWithDataDto.Status = ResponseStatus.Fail;
            responseWithDataDto.Message = response.Message;
            return responseWithDataDto;
        }
        else
        {
            responseWithDataDto.Status = ResponseStatus.Success;

            var newSleepDto = new SleepReadDto
            {
                Id = response.Data.Id,
                Start = response.Data.Start.ToString("O"),
                End = response.Data.End.ToString("O"),
                DurationHours = (response.Data.End - response.Data.Start).TotalHours.ToString()
            };

            responseWithDataDto.Data = newSleepDto;
        }

        return responseWithDataDto;
    }

    public async Task<BaseResponse<SleepReadDto>> UpdateSleep(int id, SleepUpdateDto sleepUpdateDto)
    {
        if (!DateTime.TryParse(sleepUpdateDto.Start, out var start) || !DateTime.TryParse(sleepUpdateDto.End, out var end))
        {
            return new BaseResponse<SleepReadDto>
            {
                Status = ResponseStatus.Fail,
                Message = "Invalid date format.",
                Data = null
            };
        }

        if (start >= end)
        {
            return new BaseResponse<SleepReadDto>
            {
                Status = ResponseStatus.Fail,
                Message = "Start time must be earlier than end time.",
                Data = null
            };
        }

        var updatedSleep = new Sleep
        {
            Id = id,
            Start = start,
            End = end
        };

        var response = await _sleepRepository.UpdateSleep(updatedSleep);

        if (response.Status == ResponseStatus.Fail || response.Data == null)
        {
            return new BaseResponse<SleepReadDto>
            {
                Status = ResponseStatus.Fail,
                Message = response.Message ?? "Sleep record not updated.",
                Data = null
            };
        }

        var updatedSleepDto = new SleepReadDto
        {
            Id = response.Data.Id,
            Start = response.Data.Start.ToString("O"),
            End = response.Data.End.ToString("O"),
            DurationHours = (response.Data.End - response.Data.Start).TotalHours.ToString("0")
        };

        var responseWithDataDto = new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Success,
            Data = updatedSleepDto
        };

        return responseWithDataDto;
    }

    public async Task<BaseResponse<SleepReadDto>> DeleteSleep(int id)
    {
        var response = await _sleepRepository.DeleteSleep(id);

        if (response.Status == ResponseStatus.Fail)
        {
            return new BaseResponse<SleepReadDto>
            {
                Status = ResponseStatus.Fail,
                Message = response.Message,
                Data = null
            };
        }

        return new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Success,
            Data = null
        };
    }
}
