using app.Models;
using System.Text;
using System.Text.Json;

namespace app.Services;

public class FeedingService : IFeedingService
{
    private readonly List<FeedingEvent> _feedingEvents = new();
    private readonly ILogger<FeedingService> _logger;

    public FeedingService(ILogger<FeedingService> logger)
    {
        _logger = logger;
    }

    public void RecordFeeding(FeedingEvent feedingEvent)
    {
        if (feedingEvent.Id == Guid.Empty)
        {
            feedingEvent.Id = Guid.NewGuid();
        }
        
        _feedingEvents.Add(feedingEvent);
        _logger.LogInformation($"Recorded feeding event at {feedingEvent.Timestamp}");
    }

    public List<FeedingEvent> GetAllFeedings()
    {
        return _feedingEvents.OrderByDescending(f => f.Timestamp).ToList();
    }

    public FeedingReport GenerateReport(DateTime? startDate = null, DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.MinValue;
        var end = endDate ?? DateTime.MaxValue;

        var filteredEvents = _feedingEvents
            .Where(f => f.Timestamp >= start && f.Timestamp <= end)
            .ToList();

        var report = new FeedingReport
        {
            GeneratedAt = DateTime.Now,
            TotalFeedings = filteredEvents.Count,
            AverageAmount = filteredEvents.Any() ? filteredEvents.Average(f => f.Amount) : 0,
            MostCommonBehavior = filteredEvents
                .GroupBy(f => f.FishBehavior)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "No data",
            RecentEvents = filteredEvents.OrderByDescending(f => f.Timestamp).Take(10).ToList(),
            Alerts = DetectUnusualPatterns(filteredEvents)
        };

        _logger.LogInformation($"Generated report with {report.TotalFeedings} feedings");
        return report;
    }

    public string ExportFeedingData()
    {
        var exportData = new
        {
            ExportedAt = DateTime.Now,
            TotalRecords = _feedingEvents.Count,
            FeedingEvents = _feedingEvents.OrderByDescending(f => f.Timestamp)
        };

        var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });

        _logger.LogInformation($"Exported {_feedingEvents.Count} feeding records");
        return json;
    }

    private List<string> DetectUnusualPatterns(List<FeedingEvent> events)
    {
        var alerts = new List<string>();

        if (!events.Any())
        {
            return alerts;
        }

        // Check for low feeding frequency
        if (events.Count < 2 && DateTime.Now.Subtract(events.LastOrDefault()?.Timestamp ?? DateTime.Now).Days > 2)
        {
            alerts.Add("Low feeding frequency detected - fish may not be eating enough");
        }

        // Check for excessive feeding
        var recentFeedings = events.Where(f => f.Timestamp >= DateTime.Now.AddHours(-24)).Count();
        if (recentFeedings > 5)
        {
            alerts.Add("High feeding frequency in last 24 hours - consider reducing feedings");
        }

        // Check for unusual behavior patterns
        var negativePatterns = new[] { "lethargic", "aggressive", "hiding", "not eating" };
        var concerningBehaviors = events
            .Where(f => negativePatterns.Any(p => f.FishBehavior.ToLower().Contains(p)))
            .Count();

        if (concerningBehaviors > events.Count * 0.3)
        {
            alerts.Add("Unusual behavior patterns detected - consider consulting a veterinarian");
        }

        return alerts;
    }
}
