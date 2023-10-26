using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mvc.ViewModels.Event;
using static System.Net.Mime.MediaTypeNames;

namespace mvc.Controllers;

[Route("events")]
public class EventController : Controller
{
    private readonly IHttpClientFactory _httpClient;
    private readonly IConfiguration _config;
    private readonly string? _baseUrl;
    private readonly JsonSerializerOptions _options;
    private object _context;

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
        var response = await client.GetAsync($"{_baseUrl}/events");

        if (!response.IsSuccessStatusCode) return Content("Ooops det gick fel");

        var json = await response.Content.ReadAsStringAsync();

        var events = JsonSerializer.Deserialize<IList<EventListViewModel>>(json, _options);

        return View("Index", events);
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/events/details/{id}");

        if (!response.IsSuccessStatusCode) return Content("Fel");

        var json = await response.Content.ReadAsStringAsync();
        var events = JsonSerializer.Deserialize<EventDetailsViewModel>(json, _options);

        return View("Details", events);
    }

    [HttpPost("post/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        using var client = _httpClient.CreateClient();
        var response = await client.DeleteAsync($"{_baseUrl}/events/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        return Content("Det gick inte att radera händelsen.");
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
            Id = Guid.NewGuid().ToString(),
            Title = events.Title,
            Book = events.Book,
            StartDate = events.StartDate,
            EndDate = events.EndDate,
            Description = "Test"
        };

        using var client = _httpClient.CreateClient();
        var body = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, Application.Json);

        var response = await client.PostAsync($"{_baseUrl}/events", body);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        return Content("Done!");
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/events/details/{id}");

        if (!response.IsSuccessStatusCode) return Content("Error fetching event details.");

        var json = await response.Content.ReadAsStringAsync();
        var existingEvent = JsonSerializer.Deserialize<EventEditViewModel>(json, _options);

        var events = new EventEditViewModel
        {
            Title = existingEvent.Title,
            Book = existingEvent.Book,
            Description = existingEvent.Description,
            StartDate = existingEvent.StartDate,
            EndDate = existingEvent.EndDate
        };

        return View("Edit", events);
    }

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(string id, EventEditViewModel Model)
    {
        if (!ModelState.IsValid)
            return View("Edit", Model);

        using var client = _httpClient.CreateClient();

        var responseGet = await client.GetAsync($"{_baseUrl}/events/{id}");
        if (!responseGet.IsSuccessStatusCode)
        {
            return NotFound();
        }
        var existingEventJson = await responseGet.Content.ReadAsStringAsync();
        var existingEvent = JsonSerializer.Deserialize<EventEditViewModel>(existingEventJson);

        existingEvent.Id = Model.Id;
        existingEvent.Title = Model.Title;
        existingEvent.Book = Model.Book;
        existingEvent.Description = Model.Description;
        existingEvent.StartDate = Model.StartDate;
        existingEvent.EndDate = Model.EndDate;

        var body = new StringContent(JsonSerializer.Serialize(existingEvent), Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_baseUrl}/events/{id}", body);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        return Content("Failed to update the event.");
    }
}