namespace feedbackApp.Models;

public class SeededEvents
{

    public static List<EventData> Events = new List<EventData>()
{
    new EventData { EventId= Guid.Parse("f05a5358-618c-48d4-b0e3-4c9701a8ee1b"), EventName="Azure MeetUp"},
    new EventData { EventId=Guid.Parse("8cc5acba-6412-4b37-a23b-51dfd1e77b4e"), EventName="Gen AI BootCamp"}

};

}

public class EventData
{
    public Guid EventId { get; set; }
    public string EventName { get; set; }
}