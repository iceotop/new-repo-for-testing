using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.ViewModels.Event;
using static System.Net.Mime.MediaTypeNames;

namespace mvc.Controllers;

[Route("event")]
public class EventController : Controller
{
    private readonly IHttpClientFactory _httpClient;
    private readonly IConfiguration _config;
    private readonly string? _baseUrl;
    private readonly JsonSerializerOptions _options;

    public EventController(IConfiguration config, IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
        _config = config;
        _baseUrl = _config.GetSection("apiSettings:baseUrl").Value;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
    public async Task<IActionResult> Index()
    {
        // Skapar en instans av http klienten
        using var client = _httpClient.CreateClient();
        // Hämtar datat ifrån api'et
        var response = await client.GetAsync($"{_baseUrl}/events"); // http://localhost:5120/event

        if (!response.IsSuccessStatusCode) return Content("Ooops det gick fel");

        var json = await response.Content.ReadAsStringAsync();

        var events = JsonSerializer.Deserialize<IList<EventListViewModel>>(json, _options);

        return View("Index", events);
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/events/details/{id}"); // http://localhost:5120/event/details/16
        if (!response.IsSuccessStatusCode) return Content("Fel");

        var json = await response.Content.ReadAsStringAsync();
        var events = JsonSerializer.Deserialize<EventDetailsViewModel>(json, _options);

        return View("Details", events);
    }

    [HttpGet("create")]
public IActionResult Create()
{
    var Event = new EventPostViewModel();
    return View("Create", Event);
}


    [HttpPost("create")]
    public async Task<IActionResult> Create(EventPostViewModel events)
    {
        if (!ModelState.IsValid) return View("Create", events);

        var model = new
        {
            Title = events.Title,
            Book = events.Book,
            StartDate = events.StartDate,
            EndDate = events.EndDate,
            Description = "Test"
        };

        using var client = _httpClient.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, Application.Json);

        var response = await client.PostAsync($"{_baseUrl}/events", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        return Content("Done!");
    }
}