
using System.Threading.Tasks;
using feedbackApp.Models;
using feedbackApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feedbackApp.DTOs;


namespace feedbackApp.Pages;

[Authorize(Policy ="AdminOnly")]
public class AdminDashboardModel : PageModel
{

    private readonly IDbService<UserFeedback> _feedbackDbService;
    private readonly IAdminDbService _adminDbService;

    private ILogger<AdminDashboardModel> _logger;

    // [BindProperty]
    // public IEnumerable<UserFeedback> Feedbacks {get; set;} 

    [BindProperty]
    public IEnumerable<FeedbackWithEventDTO> Feedbacks {get; set;} 


    [BindProperty(SupportsGet=true)]
    public string EventShortName 
    {
        get;
        set;
    }

   

    public AdminDashboardModel(IDbService<UserFeedback> feedbackService, ILogger<AdminDashboardModel> logger, IAdminDbService adminDbService)
    {
        _feedbackDbService = feedbackService;
        _logger = logger;
        _adminDbService = adminDbService;
    }


    public async Task<IActionResult> OnGetAsync(string eventShortName)
    {
        try
        {
           var eventData = await _adminDbService.FetchAsyncByEventShortName(eventShortName);

           
           if(eventData!=null && !string.IsNullOrEmpty(eventData.EventId) && eventData.Email==User.FindFirst("preferred_username").Value)
           {
                var entity = await _feedbackDbService.FetchAsync(eventData.EventId);
                Feedbacks = entity.Select(f => new FeedbackWithEventDTO{
                            EventId = f.EventId,
                            Name  = f.Name,
                            showNamePublicly = f.showNamePublicly,
                            Comments = f.Comments,
                            Sentiment = f.Sentiment,
                            EventName = eventData.EventName
                }).ToList();               
           }
           else 
           {
             return Forbid();
           }

           return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed in Admin Dashboard OnGetAsync.");
            return StatusCode(500, "Failed processing request admin dashboard");
        }
    }


}