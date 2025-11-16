namespace SleepTracker.Api.Models;

public class PaginationParams
{
    private const int MaxPageSize = 50;
    public int Page { get; set; } = 1;

    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public string? SortBy { get; set; }

    public bool? SortAscending { get; set; }

    public DateOnly? MinDateOfSleep { get; set; }

    public DateOnly? MaxDateOfSleep { get; set; }

    public double? MinDurationHours { get; set; }

    public double? MaxDurationHours { get; set; }
}
