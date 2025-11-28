namespace SleepTracker.Api.Models;

public class SleepDto
{
    public int Id { get; set; }

    public string Start { get; set; }

    public string End { get; set; }

    public string DurationHours { get; set; }
}
