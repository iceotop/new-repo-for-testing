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
        if (!ModelState.IsValid)
        {
            return View(model); // Return the same view to display errors
        }

        using var client = _httpClient.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"); //body

        var response = await client.PostAsync($"{_baseUrl}/account/login", content); // posta login-input till login-endpoint i api

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync(); // läser ut API-response som innehåller JWT
            var userViewModel = JsonSerializer.Deserialize<UserViewModel>(jsonResponse); // deserialisera json-responsen i UserViewModel. OBS case-sensitive

            // Lagra JWT i en säker cookie med namnet "access_token"
            Response.Cookies.Append("access_token", userViewModel.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return RedirectToAction("Profile");
        }
        
        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return View("Login", model);
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

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View("Register");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        using var client = _httpClient.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{_baseUrl}/account/register", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        // Read the response content and deserialize it into a dictionary
        var errorContent = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(errorContent);
        var errorsElement = jsonDoc.RootElement.GetProperty("errors");
        var errors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(errorsElement.GetRawText());

        // Add the errors to the ModelState
        if (errors != null)
        {
            foreach (var error in errors)
            {
                foreach (var detail in error.Value)
                {
                    ModelState.AddModelError(error.Key, detail);
                }
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Registration failed");
        }

        return View("Register", model);
    }


    [HttpGet("logout")]
    public IActionResult Logout()
    {
        // Remove the JWT token cookie
        Response.Cookies.Delete("access_token");

        // Remove the anti-forgery cookie
        Response.Cookies.Delete(".AspNetCore.Antiforgery.rlXYCFrAyI4");

        // Redirect to home page or login page
        return RedirectToAction("Index", "Home");
    }


}




    // [HttpGet("details/{id}")]
    // public async Task<IActionResult> Details(string id)
    // {
    //     using var client = _httpClient.CreateClient();
    //     var response = await client.GetAsync($"{_baseUrl}/account/{id}");
    //     if (!response.IsSuccessStatusCode) return Content("Fel");

    //     var json = await response.Content.ReadAsStringAsync();
    //     var profile = JsonSerializer.Deserialize<ProfileViewModel>(json, _options);

    //     return View("Profile", profile);
    // }