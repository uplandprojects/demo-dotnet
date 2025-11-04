namespace app.Models;

public class FeedingReport
{
    public DateTime GeneratedAt { get; set; }
    public int TotalFeedings { get; set; }
    public double AverageAmount { get; set; }
    public string MostCommonBehavior { get; set; } = string.Empty;
    public List<string> Alerts { get; set; } = new();
    public List<FeedingEvent> RecentEvents { get; set; } = new();
}
