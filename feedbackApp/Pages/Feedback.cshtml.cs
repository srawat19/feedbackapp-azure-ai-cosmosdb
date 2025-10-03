using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feedbackApp.Models;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using feedbackApp.Services;
using Azure.AI.TextAnalytics;
using System.Net;

namespace feedbackApp.Pages;

public class FeedbackModel : PageModel

{
    public List<RatingOptions> Ratings { get; set; }

    [BindProperty]
    public UserFeedback UserFeedback { get; set; } = new UserFeedback();

    public EventData CurrentEvent { get; set; }

    
    [BindProperty(SupportsGet = true)]
    public Guid EventId {get; set;} 

    private CosmosClient cosmosClient;
    private Container container;
    private readonly ILogger<FeedbackModel> _logger;

    private readonly SentimentService _sentimentService;

    private readonly IDbService<UserFeedback> _feedbackDbService;


    public FeedbackModel(ILogger<FeedbackModel> logger, SentimentService sentimentService, IDbService<UserFeedback> feedbackDbService)
    {
        _logger = logger;
        Ratings = new List<RatingOptions>();
        _sentimentService = sentimentService;
        _feedbackDbService = feedbackDbService;

    }


    public void OnGet()
    {
        try
        {
            //Setting EventId
            CurrentEvent = SeededEvents.Events?.FirstOrDefault(e => e.EventId == EventId)!;

            if (CurrentEvent != null)
            {
                UserFeedback.EventId = CurrentEvent.EventId.ToString();
            }

            Ratings.Add(new RatingOptions() { Value = 1, Emoji = "😢", Description = "Didn't Like!" });
            Ratings.Add(new RatingOptions() { Value = 2, Emoji = "😐", Description = "Ok!" });
            Ratings.Add(new RatingOptions() { Value = 3, Emoji = "🙂", Description = "Good!" });
            Ratings.Add(new RatingOptions() { Value = 4, Emoji = "😊", Description = "Liked It!" });
            Ratings.Add(new RatingOptions() { Value = 5, Emoji = "😍💯", Description = "Loved It!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed in Feedback OnGet().");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            UserFeedback.Id = Guid.NewGuid().ToString();


            if (ModelState != null)
            {
                ModelState["UserFeedback.Id"]?.Errors.Clear();
                ModelState.Remove("UserFeedback.Id");
            }

            _logger.LogInformation($"UserFeedback Id is {UserFeedback.Id}");

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            _logger.LogError($"ModelState Error for {key}: {error.ErrorMessage}");

                        }
                    }
                }
                return Page();
            }

            // Get the sentimental Analysis done
            if (!string.IsNullOrEmpty(UserFeedback.Comments))
            {
                DocumentSentiment commentSentiments = await _sentimentService.GetSentimentAsync(UserFeedback.Comments);
                UserFeedback.Sentiment = commentSentiments.Sentiment.ToString();
                _logger.LogInformation($"Review comment sentiment analysis is done. Sentiment Value : {UserFeedback.Sentiment}");
            }
            else
            {
                _logger.LogError("Feedback comments was null. ");
            }

            // Save to CosmosDb
            UserFeedback.SubmittedOn = DateTime.UtcNow;

            _logger.LogInformation("Going for saving Review comment in cosmos");

            var response = await _feedbackDbService.SaveAsync(UserFeedback);
            if (response == HttpStatusCode.Created)
            {
                //await container.CreateItemAsync<UserFeedback>(UserFeedback, new PartitionKey(UserFeedback.EventId));
            _logger.LogInformation("Save was successful.");

                return RedirectToPage("ThankYou", new { eventId = UserFeedback.EventId });
            }
            else
            {
                _logger.LogError("Failed to save user feedback.");
                ModelState.AddModelError("", $"Failed with Error Code : {response}");
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed in Feedback OnPostAsync().");
            return StatusCode(500,"Error occured in Feedback");
        }
    }
}

