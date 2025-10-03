namespace feedbackApp.Models;

public class RatingOptions
{
    public int Value { get; set; } //1-5
    public string Emoji { get; set; } //emojis
    public string Description { get; set; } //Describes what emojis mean
}