using Microsoft.AspNetCore.Mvc;

namespace mvc.Controllers
{
    [Route("event")]
    public class EventController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }

        [Route("details")] // Här ska routing ske via ID – endast för demo
        public IActionResult Details(){
            return View("Details");
        }
    }
}