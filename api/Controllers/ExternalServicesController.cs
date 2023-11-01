using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using mvc.Controllers;
using static api.Models.DTOs.ExternalServicesController;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/externalservices")]
    public partial class ExternalServicesController : ControllerBase
    {
        private readonly IHttpClientFactory  _httpClient;
        private readonly JsonSerializerOptions _options;

        public ExternalServicesController(IHttpClientFactory httpClient)
        {
             _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string query)
        {
            var client = _httpClient.CreateClient("GoogleBooks");
            var response = await client.GetAsync($"volumes?q={Uri.EscapeDataString(query)}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to retrieve data from external API.");
            }
            var responseStream = await response.Content.ReadAsStreamAsync();

            try
            {
                var googleBooksResponse = await JsonSerializer.DeserializeAsync<GoogleBooksResponse>(responseStream, _options);
                var books = googleBooksResponse?.Items.Select(item => new BookDto {
                    Id = item.Id,
                    Title = item.VolumeInfo.Title,
                    Author = item.VolumeInfo.Authors?.FirstOrDefault(),
                    PublicationYear = item.VolumeInfo.PublishedDate,
                    ImageUrl = item.VolumeInfo.ImageLinks?.Thumbnail
                }).ToList();

                return Ok(books);
            }
            catch (JsonException ex)
            {
                return BadRequest("Failed to deserialize the response from the external API: " + ex.Message);
            }
        }

        [HttpGet("book/{id}")]
        public async Task<IActionResult> GetBookById(string id)
        {
            var client = _httpClient.CreateClient("GoogleBooks");
            var response = await client.GetAsync($"volumes/{Uri.EscapeDataString(id)}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to retrieve data from external API.");
            }
            var responseStream = await response.Content.ReadAsStreamAsync();

            try
            {
                var googleBookResponse = await JsonSerializer.DeserializeAsync<GoogleBookItem>(responseStream, _options);
                var book = new BookDto
                {
                    Title = googleBookResponse.VolumeInfo.Title,
                    Author = googleBookResponse.VolumeInfo.Authors?.FirstOrDefault(),
                    PublicationYear = googleBookResponse.VolumeInfo.PublishedDate,
                    ImageUrl = googleBookResponse.VolumeInfo.ImageLinks?.Thumbnail
                };

                return Ok(book);
            }
            catch (JsonException ex)
            {
                return BadRequest("Failed to deserialize the response from the external API: " + ex.Message);
            }
        }
    }
}