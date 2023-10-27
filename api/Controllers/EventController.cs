using api.Data;
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
    public EventController(BookCircleContext context)
    {
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
    public async Task<ActionResult> GetById(string id)
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
    public async Task<ActionResult> Create(EventBaseViewModel model)
    {
        var eventToAdd = new Event
        {
            Id = Guid.NewGuid().ToString(),
            Title = model.Title,
            Book = model.Book,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };

        await _context.Events.AddAsync(eventToAdd);

        if (await _context.SaveChangesAsync() > 0)
        {
            return Created(nameof(GetById), new { id = eventToAdd.Id });
        }

        return StatusCode(500, "Internal Server Error");
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

    [HttpPost("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Edit(string id, Event model)
    {
        var bookEvent = await _context.Events.FindAsync(id);
        if (bookEvent == null)
            return BadRequest("Event not found.");

        bookEvent.Id = model.Id;
        bookEvent.Title = model.Title;
        bookEvent.Description = model.Description;
        bookEvent.StartDate = model.StartDate;
        bookEvent.EndDate = model.EndDate;

        _context.Events.Update(bookEvent);

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