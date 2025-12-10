namespace SleepTracker.Api.Models;

public class SleepCreateDto
{
    public string Start { get; set; }

    public string End { get; set; }

    public bool IsDeleted { get; set; } = false;
}
