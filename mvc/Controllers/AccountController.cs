using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mvc.ViewModels.Account;

namespace mvc.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClient;
    private readonly IConfiguration _config;
    private readonly string? _baseUrl;
    private readonly JsonSerializerOptions _options;

    public AccountController(IConfiguration config, IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
        _config = config;
        _baseUrl = _config.GetSection("apiSettings:baseUrl").Value;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        using var client = _httpClient.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/account/{id}");
        if (!response.IsSuccessStatusCode) return Content("Fel");

        var json = await response.Content.ReadAsStringAsync();
        var profile = JsonSerializer.Deserialize<ProfileViewModel>(json, _options);

        return View("Profile", profile);
    }

}