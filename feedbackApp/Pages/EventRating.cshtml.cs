using System.Net;
using feedbackApp.Models;
using feedbackApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feedbackApp.Pages;

/// <summary>
/// Will retrieve Review Sentiment from Cosmos DB, Average it and who in UI. 
/// EventId is passed as route-id
/// </summary>
public class EventRatingModel : PageModel
{

    private readonly CosmosDbService _cosmosDbService;
    private readonly ILogger<EventRatingModel> _logger;

    public string EventAvgSentiment { get; set; }

    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; }   //this denoted your eventID



    public EventRatingModel(CosmosDbService cosmosDbService, ILogger<EventRatingModel> logger)
    {
        _cosmosDbService = cosmosDbService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? Id)
    {
        try
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest("EventId was missing.");
            }

            _logger.LogInformation("Processing OnGetAsync()");
            IEnumerable<UserFeedback> eventFeedbacksList = await _cosmosDbService.FetchAsync(Id);
            TotalReviews = eventFeedbacksList.Count();

            //Fetch the avg score/rating here
            //commenting below call and performing the calculation here itself to reduce cosmos DB call.
            //AverageRating = await _cosmosDbService.GetEventsAverageScoreAsync(Id);

            if (eventFeedbacksList != null && eventFeedbacksList.Any())
            {
                AverageRating = eventFeedbacksList.Average(e => e.Rating);
                AverageRating = Math.Round(AverageRating,1);
            }


            if (AverageRating == 0)
            {
                EventAvgSentiment = "Waiting for the feedbacks. ⏳";
                return Page();
            }
            if (AverageRating >= 4.0)
                EventAvgSentiment = "Attendees loved this event ! 😄";
            else if (AverageRating >= 2.5)
                EventAvgSentiment = "Overall mixed reviews. 😐 ";
            else
                EventAvgSentiment = "Looks like event didn't meet attendees expectations. 😢";

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed in EventRating OnGetAsync.");
            return StatusCode(500,"Error occurred in EventRating OnGetAsync.");
        }
    }
}