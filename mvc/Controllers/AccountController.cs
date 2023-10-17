using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace mvc.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _config;
        private readonly JsonSerializerOptions _options;
        private readonly string? _baseUrl;
        public AccountController(IConfiguration config, IHttpClientFactory httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _baseUrl = _config.GetSection("apiSettings:baseUrl").Value;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}