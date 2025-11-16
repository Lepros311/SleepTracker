namespace SleepTracker.Api.Models;

public class Sleep
{
    public int Id { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public TimeSpan DurationHours => End - Start;

    public bool IsDeleted { get; set; } = false;
}
