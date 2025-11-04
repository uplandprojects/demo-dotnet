namespace app.Models;

public class FeedingEvent
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string FoodType { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string FishBehavior { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
