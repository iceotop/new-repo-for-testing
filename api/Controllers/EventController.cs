using api.Data;
using api.Interfaces;
using api.Models;
using api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Api.Controllers;

[Route("api/v1/events")]
[ApiController]
public class EventController : ControllerBase
{
    public BookCircleContext _context;
    private readonly IEventRepository _repo;
    public EventController(BookCircleContext context, IEventRepository repo)
    {
        _repo = repo;
        _context = context;
    }

    // TODO Här skulle vi behöva fixa en ViewModel
    [HttpGet]
    public async Task<ActionResult<List<Event>>> Get()
    {
        return Ok(await _context.Events.ToListAsync());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<Event>> Get(string id)
    {
        var result = await _context.Events
            .Where(e => e.Id == id)
            .Select(e => new EventBaseViewModel
            {
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Books = e.Books!.Select(
                    b => new BookBaseViewModel
                    {
                        Title = b.Title,
                        Author = b.Author,
                        PublicationYear = b.PublicationYear,
                        Review = b.Review,
                        IsRead = b.IsRead
                    }
                ).ToList()
            }).SingleOrDefaultAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<List<Event>>> Add(Event newEvent)
    {
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        return Ok(await _context.Events.ToListAsync());
    }

    [HttpPut]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<List<Event>>> Update(Event request)
    {
        var bookEvent = await _context.Events.FindAsync(request.Id);
        if (bookEvent == null)
            return BadRequest("Event not found.");

        bookEvent.Title = request.Title;
        bookEvent.Book = request.Book;
        bookEvent.Description = request.Description;
        bookEvent.StartDate = request.StartDate;
        bookEvent.EndDate = request.EndDate;

        await _context.SaveChangesAsync();

        return Ok(await _context.Events.ToListAsync());
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<Event>>> Delete(string id)
    {
        var bookEvent = await _context.Events.FindAsync(id);
        if (bookEvent == null)
            return BadRequest("Event not found.");

        _context.Events.Remove(bookEvent);

        await _context.SaveChangesAsync();
        return Ok(await _context.Events.ToListAsync());
    }

    [HttpGet("details/{id}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<Event>> Details(string id)
    {
        var bookEvent = await _context.Events.FindAsync(id);

        if (bookEvent == null)
            return NotFound("Event not found.");

        return Ok(bookEvent);
    }

    [HttpPatch("join/{eventId}/{userId}")]
    public async Task<IActionResult> Join(string eventId, string userId)
    {
        var e = await _context.Events.FindAsync(eventId);
        if (e is null) return NotFound($"Bokcirkel med ID {eventId} kunde inte hittas");

        var user = await _context.Users.FindAsync(userId);
        if (user is null) return NotFound($"Användare med ID {userId} kunde inte hittas");

        user.Events.Add(e);
        _context.Users.Update(user);

        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

}