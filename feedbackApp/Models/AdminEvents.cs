using Newtonsoft.Json;

namespace feedbackApp.Models;

public class AdminEvents
{
    
    //JsonProperty has been used here to ensure properties here match Cosmos fields exactly
    // else deserialization will have problems(getting data from cosmos db will return empty objects)
    //

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("eventId")]
    public string EventId
    {
        get; set;
    }

    [JsonProperty("eventName")]
    public string EventName { get; set; }

    [JsonProperty("eventShortName")]
    public string EventShortName { get; set; }

    [JsonProperty("role")]
    public string Role { get; set; }
}