using app.Models;

namespace app.Services;

public interface IFeedingService
{
    void RecordFeeding(FeedingEvent feedingEvent);
    List<FeedingEvent> GetAllFeedings();
    FeedingReport GenerateReport(DateTime? startDate = null, DateTime? endDate = null);
    string ExportFeedingData();
}
