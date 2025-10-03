using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace feedbackApp.Models;
public class UserFeedback
{
    [BindNever]
    [JsonProperty("id")]
    public string Id { get; set; } //unique id for cosmosdb

    [JsonProperty("eventId")]
    public string EventId { get; set; } //unique id for events

    public string? Name { get; set; }
    public bool showNamePublicly { get; set; }

    public string? Comments { get; set; }

    public string? Sentiment { get; set; }

    public int Rating { get; set; } //rating via emojis but we will store in DB numeric for consistency.
    
    public DateTime SubmittedOn { get; set; }
}