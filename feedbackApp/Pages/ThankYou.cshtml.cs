
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feedbackApp.Pages;

public class ThankYouModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string EventId { get; set; }

    private readonly ILogger<ThankYouModel> _logger;


    public ThankYouModel(ILogger<ThankYouModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation($"Thank you page accessed for EventId: {EventId}");
    }
}
