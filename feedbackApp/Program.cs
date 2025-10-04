using feedbackApp.Services;
using Microsoft.Azure.Cosmos;
using Azure.Identity;
using Microsoft.Extensions.Options;
using feedbackApp.Models;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

//Adding Authentication for Admin role.

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
});

builder.Services.AddAuthorization(option =>
    { option.AddPolicy("AdminOnly", policy =>  policy.RequireRole("Admin"));});

// builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
// {
//     options.TokenValidationParameters.RoleClaimType = "roles"; // Azure AD sends app roles in "roles" claim
// });
// Add services to the container.
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

builder.Services.Configure<CosmosDbConfiguration>(builder.Configuration.GetSection("CosmosDb"));

builder.Services.AddSingleton<CosmosClient>(s =>
{
    var config = s.GetRequiredService<IOptions<CosmosDbConfiguration>>().Value;
   // var config = s.GetRequiredService<IOptionsSnapshot<CosmosDbConfiguration>>().Value; -- for each request this gets refreshed.
    //var config = s.GetRequiredService<IOptionsMonitor<CosmosDbConfiguration>>().CurrentValue;

    return new CosmosClient(config.Endpoint, new DefaultAzureCredential());
});

//Injecting cosmos DB Service.
//builder.Services.AddScoped(typeof(IDbService<UserFeedback>), typeof(CosmosDbService));
builder.Services.AddScoped<IDbService<UserFeedback>,CosmosDbService>();
builder.Services.AddScoped<IDbService<AdminEvents>,AdminDbService>();
builder.Services.AddScoped<IAdminDbService, AdminDbService>();


//Adding CosmosDbService for concrete injection
builder.Services.AddScoped<CosmosDbService>();

//Adding Sentiment Service which utilizes Azure AI Text Analytics
builder.Services.AddSingleton<SentimentService>();

builder.Logging.AddConsole(); // Logs to console → captured by App Service
builder.Logging.AddDebug(); // Logs to debug output → captured by App Service
builder.Logging.AddAzureWebAppDiagnostics(); // // Enables Azure log streaming/log files

var app = builder.Build();
var port = Environment.GetEnvironmentVariable("PORT") ?? "7081";

app.Urls.Add($"http://+:{port}"); //+ allows binding to all network interfaces
// Uncomment this line if you want to bind to localhost only
//builder.WebHost.UseUrls($"http://*:{port}", $"https://*:{port}"); // Uncomment this line if you want to bind to all network interfaces



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/", context =>
{
    var events = SeededEvents.Events.Select(x=>x.EventId).ToList();
    Random random = new Random();
    string eventId = events[random.Next(events.Count())].ToString();
    context.Response.Redirect($"/Feedback?eventId={eventId}");
    return Task.CompletedTask;
});
app.Run();
