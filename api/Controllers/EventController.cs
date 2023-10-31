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
    private readonly IEventRepository _eventRepo;
    private readonly IUserRepository _userRepo;
    public EventController(BookCircleContext context, IEventRepository eventRepo, IUserRepository userRepo)
    {
        _userRepo = userRepo;
        _eventRepo = eventRepo;
        _context = context;
    }

    // TODO Här skulle vi behöva fixa en ViewModel
    [HttpGet]
    public async Task<ActionResult<List<Event>>> Get()
    {
        return Ok(await _eventRepo.ListAllAsync());
    }

    [HttpGet("{id}")]
    // [Authorize(Roles = "User")]
    public async Task<ActionResult<Event>> GetById(string id)
    {
        // var result = await _context.Events
        var result = await _eventRepo.FindByIdAsync(id);

        // .Where(e => e.Id == id)
        var bookEvent = new EventBaseViewModel
        {
            Title = result.Title,
            Description = result.Description,
            StartDate = result.StartDate,
            EndDate = result.EndDate,
            Books = result.Books!.Select(
                b => new BookBaseViewModel
                {
                    Title = b.Title,
                    Author = b.Author,
                    PublicationYear = b.PublicationYear,
                    Review = b.Review,
                    IsRead = b.IsRead
                }
            ).ToList()
        };
        //  }).SingleOrDefaultAsync();
        // return Ok(bookEvent);
        return Ok(result);
    }

    // [HttpPost]
    // [Authorize(Roles = "User")]
    // public async Task<ActionResult<List<Event>>> Add(Event newEvent)
    // {
    //     _context.Events.Add(newEvent);
    //     await _context.SaveChangesAsync();

    //     return Ok(await _context.Events.ToListAsync());
    // }

    [HttpPost]
    // [Authorize(Roles = "User")]
    public async Task<ActionResult<List<Event>>> Add(EventPostViewModel newEvent)
    {
        var bookEvent = new Event
        {
            Id = Guid.NewGuid().ToString(),
            Title = newEvent.Title,
            Book = newEvent.Book,
            Description = newEvent.Description,
            StartDate = newEvent.StartDate,
            EndDate = newEvent.EndDate,
        };

        // var result = await _eventRepo.AddAsync(bookEvent);

        await _eventRepo.AddAsync(bookEvent);

        if (await _eventRepo.SaveAsync())
        {
            return Created(nameof(GetById), new { id = bookEvent.Id });
        }
        return StatusCode(500, "Internal Server Error");

        // _context.Events.Add(newEvent);
        // await _context.SaveChangesAsync();

        // return Ok(await _context.Events.ToListAsync());
    }

    [HttpPut]
    // [Authorize(Roles = "User")]
    public async Task<ActionResult<List<Event>>> Update(Event request)
    {
        // var bookEvent = await _context.Events.FindAsync(request.Id);
        var bookEvent = await _eventRepo.FindByIdAsync(request.Id);
        if (bookEvent == null)
            return BadRequest("Event not found.");

        bookEvent.Title = request.Title;
        bookEvent.Book = request.Book;
        bookEvent.Description = request.Description;
        bookEvent.StartDate = request.StartDate;
        bookEvent.EndDate = request.EndDate;


        await _eventRepo.SaveAsync();

        return Ok(await _eventRepo.ListAllAsync());
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<Event>>> Delete(string id)
    {
        var bookEvent = await _eventRepo.FindByIdAsync(id);
        if (bookEvent == null)
            return BadRequest("Event not found.");

        // _context.Events.Remove(bookEvent);
        await _eventRepo.DeleteAsync(bookEvent);
        await _eventRepo.SaveAsync();

        return Ok(await _eventRepo.ListAllAsync());
    }

    // Gör om till ViewModel
    // [HttpGet("details/{id}")]
    // [Authorize(Roles = "User")]
    // public async Task<ActionResult<Event>> Details(string id)
    // {
    //     var bookEvent = await _context.Events.FindAsync(id);

    //     if (bookEvent == null)
    //         return NotFound("Event not found.");

    //     return Ok(bookEvent);
    // }

    [HttpPatch("join/{eventId}/{userId}")]
    public async Task<IActionResult> Join(string eventId, string userId)
    {
        // var e = await _context.Events.FindAsync(eventId);
        var e = await _eventRepo.FindByIdAsync(eventId);
        if (e is null) return NotFound($"Bokcirkel med ID {eventId} kunde inte hittas");

        var user = await _userRepo.FindByIdAsync(userId);
        if (user is null) return NotFound($"Användare med ID {userId} kunde inte hittas");

        user.Events.Add(e);
        await _userRepo.UpdateAsync(user);

        if (await _userRepo.SaveAsync())
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }
}