using System.Net.Http.Headers;
using System.Text;
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


    public AccountController(
        IConfiguration config,
        IHttpClientFactory httpClient)
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

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View("Login");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        using var client = _httpClient.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_baseUrl}/account/login", content); // posta login-input till login-endpoint i api

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync(); // läser ut API-response som innehåller JWT
            var userViewModel = JsonSerializer.Deserialize<UserViewModel>(jsonResponse); // deserialisera json-responsen i UserViewModel. OBS case-sensitive

            // Lagra JWT i en säker cookie med namnet "access_token"
            Response.Cookies.Append("access_token", userViewModel.token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return RedirectToAction("Profile");
        }
        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return RedirectToAction("Login");
    }

    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var token = Request.Cookies["access_token"]; // plocka fram token ur cookie

        using var client = _httpClient.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); // slänger på token i request-headern
        var response = await client.GetAsync($"{_baseUrl}/account/email"); // GET-endpointen i API har nu även funktionalitet som hanterar requests med JWT
        if (!response.IsSuccessStatusCode) return Content("Fel");

        var json = await response.Content.ReadAsStringAsync();
        var profile = JsonSerializer.Deserialize<ProfileViewModel>(json, _options);
        return View("Profile", profile);
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