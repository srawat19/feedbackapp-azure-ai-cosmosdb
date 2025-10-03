using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feedbackApp.Services;
using System.Security.Claims;
using System.Linq;
using feedbackApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace feedbackApp.Pages;

[Authorize(Policy="AdminOnly")]
public class AdminHomeModel : PageModel
{
    private readonly ILogger<AdminHomeModel> _logger;
    private readonly IDbService<AdminEvents> _adminService;

    [BindProperty]
    public IEnumerable<AdminEvents> Events {get; set;} 
    //= new List<AdminEvents>();


    public AdminHomeModel(ILogger<AdminHomeModel> logger, IDbService<AdminEvents> adminService)
    {
        _logger = logger;
        _adminService = adminService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {       
            _logger.LogInformation("Processing AdminHome OnGetAsync()");
            string email = User.FindFirst("preferred_username").Value;
            Events =  await _adminService.FetchAsync(email);
            return Page();
        }
        catch(Exception ex)
        {
            _logger.LogError("Error occured when processing AdminHome OnGetAsync()");
            return StatusCode(500,"Error occured while processing the Admin request.");
        }
    }
}