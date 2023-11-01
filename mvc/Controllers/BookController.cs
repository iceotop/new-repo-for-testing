using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mvc.ViewModels.Book;
using mvc.ViewModels.Search;

namespace mvc.Controllers
{
    [Route("[controller]")]
    public class BookController : Controller
    {

        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _config;
        private readonly string? _baseUrl;
        private readonly JsonSerializerOptions _options;

        public BookController (IConfiguration config, IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
            _config = config;
            _baseUrl = _config.GetSection("apiSettings:baseUrl").Value;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public IActionResult Index()
        {
            return View("Index");
        }
        
        [HttpGet("create")]
        public async Task<IActionResult> Create(string id)
        {
            using var client = _httpClient.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}/externalservices/book/{id}");

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var book = await JsonSerializer.DeserializeAsync<BookDetailsViewModel>(responseStream, _options);

                return View("Create", book);
            }
            else
            {
                return View("_Error");
            }
        }


        [HttpPost("create")]
        public async Task<IActionResult> Create(BookPostViewModel book)
        {
            // Read the token from the cookie
            var token = Request.Cookies["access_token"];

            // Create an HTTP client
            using var client = _httpClient.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            // Create the HTTP content
            var content = new StringContent(JsonSerializer.Serialize(book), Encoding.UTF8, "application/json");

            // Send the POST request to your API
            var response = await client.PatchAsync($"{_baseUrl}/books/addtolibrary", content);

            if (response.IsSuccessStatusCode)
            {
                // Redirect to the profile page
                return RedirectToAction("Profile", "Account");
            }
            else
            {
                // Handle the error
                return View("_Error");
            }
        }

    }
}

// if (!ModelState.IsValid) return View("Create", events);

            // var model = new
            // {
            //     Title = events.Title,
            //     Book = events.Book,
            //     StartDate = events.StartDate,
            //     EndDate = events.EndDate,
            //     Description = "Test"
            // };

            // using var client = _httpClient.CreateClient();
            // var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, Application.Json);

            // var response = await client.PostAsync($"{_baseUrl}/events", content);

            // if (response.IsSuccessStatusCode)
            // {
            //     return RedirectToAction(nameof(Index));
            // }
            // return Content("Done!");