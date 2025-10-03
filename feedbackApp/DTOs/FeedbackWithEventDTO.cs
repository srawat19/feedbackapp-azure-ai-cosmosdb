using System;

namespace feedbackApp.DTOs
{

public class FeedbackWithEventDTO
{

    public string EventId { get; set; } //unique id for events

    public string? Name { get; set; }

    public string? EventName { get; set; }

    
    public bool showNamePublicly { get; set; }

    public string? Comments { get; set; }

    public string? Sentiment { get; set; }

}
}