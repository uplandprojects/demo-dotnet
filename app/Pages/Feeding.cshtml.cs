using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using app.Models;
using app.Services;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace app.Pages;

public class FeedingModel : PageModel
{
    private readonly IFeedingService _feedingService;
    private readonly ILogger<FeedingModel> _logger;

    [BindProperty]
    [Required]
    public string FoodType { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Range(0.01, 1000.0, ErrorMessage = "Amount must be between 0.01 and 1000 grams")]
    public double Amount { get; set; }

    [BindProperty]
    [Required]
    public string FishBehavior { get; set; } = string.Empty;

    [BindProperty]
    public string Notes { get; set; } = string.Empty;

    public FeedingReport? Report { get; set; }

    public FeedingModel(IFeedingService feedingService, ILogger<FeedingModel> logger)
    {
        _feedingService = feedingService;
        _logger = logger;
    }

    public void OnGet()
    {
        Report = _feedingService.GenerateReport();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            Report = _feedingService.GenerateReport();
            return Page();
        }

        var feedingEvent = new FeedingEvent
        {
            Timestamp = DateTime.Now,
            FoodType = FoodType,
            Amount = Amount,
            FishBehavior = FishBehavior,
            Notes = Notes
        };

        _feedingService.RecordFeeding(feedingEvent);
        _logger.LogInformation($"Feeding event recorded: {FoodType}, {Amount}g");

        return RedirectToPage();
    }

    public IActionResult OnPostExport()
    {
        var exportData = _feedingService.ExportFeedingData();
        var bytes = Encoding.UTF8.GetBytes(exportData);
        
        return File(bytes, "application/json", $"feeding-data-{DateTime.Now:yyyyMMdd}.json");
    }
}
