using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BookController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult GetById(int id)
    {
        return Ok();
    }

    [HttpPost()]
    public ActionResult Create()
    {
        return Created(nameof(GetById), new { message = "Created {id}" });
    }
}