namespace SleepTracker.Api.Responses;

public class PagedResponse<T>
{
    public ResponseStatus Status { get; set; }

    public string Message { get; set; }

    public T? Data { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public PagedResponse(T? data, int pageNumber, int pageSize, int totalRecords)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
    }

    public static PagedResponse<T> Success(T data, int pageNumber, int pageSize, int totalRecords)
    {
        return new PagedResponse<T>(data, pageNumber, pageSize, totalRecords)
        {
            Status = ResponseStatus.Success
        };
    }

    public static PagedResponse<T> Fail(string message)
    {
        return new PagedResponse<T>(default, 0, 0, 0)
        {
            Status = ResponseStatus.Fail,
            Message = message
        };
    }
}
