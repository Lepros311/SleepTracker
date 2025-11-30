namespace SleepTracker.Api.Models;

public class SleepReadDto
{
    public int Id { get; set; }

    public string Start { get; set; }

    public string End { get; set; }

    public string DurationHours { get; set; }
}
