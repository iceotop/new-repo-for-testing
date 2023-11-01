using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mvc.ViewModels;
using mvc.ViewModels.Search;

namespace mvc.Controllers
{
    [Route("[controller]")]
    public class SearchController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _config;
        private readonly string? _baseUrl;
        private readonly JsonSerializerOptions _options;

        public SearchController(IConfiguration config, IHttpClientFactory httpClient)
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

        [HttpGet("BookResults")]
        public async Task<IActionResult> BookResults(string query)
        {
            using var client = _httpClient.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}/externalservices/search?query={Uri.EscapeDataString(query)}");

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                
                // Deserialize directly into a list of SearchResultsViewModel
                var books = await JsonSerializer.DeserializeAsync<List<SearchResultsViewModel>>(responseStream, _options);

                if (books == null)
                {
                    books = new List<SearchResultsViewModel>(); // Initialize to empty list if null
                }

                return View("BookResults", books);
            }
            else
            {
                // Handle error
                return View("_Error");
            }
        }

    }

}
