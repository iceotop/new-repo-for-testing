using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/externalservices")]
    public class ExternalServicesController : ControllerBase
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

        public class BookDto
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string PublicationYear { get; set; }
            public string ImageUrl { get; set; }
        }


//------classes to handle response from google books api
        public class GoogleBooksResponse
        {
            public List<GoogleBookItem> Items { get; set; }
        }

        public class GoogleBookItem
        {
            public VolumeInfo VolumeInfo { get; set; }
        }

        public class VolumeInfo
        {
            public string Title { get; set; }
            public List<string> Authors { get; set; }
            public string PublishedDate { get; set; }
            public ImageLinks ImageLinks { get; set; }
        }

        public class ImageLinks
        {
            public string Thumbnail { get; set; }
        }
    }
}