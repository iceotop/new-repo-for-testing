using api.Data;
using api.Interfaces;
using api.Models;
using api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace api.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BookController : ControllerBase
{
    // private readonly BookCircleContext _context;
    private readonly IBookRepository _bookRepo;
    private readonly IUserRepository _userRepo;
    private readonly IEventRepository _eventRepo;

    // public BookController(BookCircleContext context, IBookRepository bookRepo, IUserRepository userRepo, IEventRepository eventRepo)
    public BookController(IBookRepository bookRepo, IUserRepository userRepo, IEventRepository eventRepo)
    {
        _eventRepo = eventRepo;
        _userRepo = userRepo;
        _bookRepo = bookRepo;
        // _context = context;
    }

    // Display all books
    [HttpGet]
    public async Task<IActionResult> ListAllBooks()
    {
        // var list = await _context.Books.ToListAsync();
        var list = await _bookRepo.ListAllAsync();

        if (list is []) return NotFound($"Böcker kunde inte hittas");

        return Ok(list);
    }

    // Hämta enskild bok på ID
    [HttpGet("{id}")]
    // [Authorize(Roles = "User")]
    // [Authorize(Roles = "User, Admin")] - Om man vill lägga till flera roller
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _bookRepo.FindByIdAsync(id);

        // Eventuell felhantering
        //     if (book == null)
        // {
        //     return NotFound($"Boken med ID {id} kunde inte hittas");
        // }

        var book = new
        {
            Id = result.Id,
            Title = result.Title,
            Author = result.Author,
            PublicationYear = result.PublicationYear,
            Review = result.Review,
            IsRead = result.IsRead
        };

        // var result = await books.Select(b => new
        // {
        //     Id = b.Id,
        //     Title = b.Title,
        //     Author = b.Author,
        //     PublicationYear = b.PublicationYear,
        //     Review = b.Review,
        //     IsRead = b.IsRead
        // })
        // .SingleOrDefaultAsync(b => b.Id == id);

        // if (result is null) return BadRequest($"Boken med ID {id} kunde inte hittas");

        return Ok(book);
    }

    // Add a new book
    [HttpPost()]
    // [Authorize(Roles = "User")]
    public async Task<ActionResult> Create(BookBaseViewModel model)
    {
        var book = new Book
        {
            Id = Guid.NewGuid().ToString(),
            Title = model.Title,
            Author = model.Author,
            PublicationYear = model.PublicationYear,
            Review = model.Review,
            IsRead = model.IsRead
        };

        // await _context.Books.AddAsync(book);
        await _bookRepo.AddAsync(book);

        if (await _bookRepo.SaveAsync())
        {
            return Created(nameof(GetById), new { id = book.Id });
        }
        return StatusCode(500, "Internal Server Error");
    }

    // Edit a book
    [HttpPut("{id}")]
    // [Authorize(Roles = "User")]
    public async Task<IActionResult> EditBook(string id, [FromBody] Book updatedBook)
    {
        // var existingBook = await _context.Books.FindAsync(id);
        var existingBook = await _bookRepo.FindByIdAsync(id);

        if (existingBook is null) return NotFound($"Bok ({id}) finns inte i systemet");

        existingBook.Title = updatedBook.Title;
        existingBook.Author = updatedBook.Author;
        existingBook.PublicationYear = updatedBook.PublicationYear;
        existingBook.Review = updatedBook.Review;
        existingBook.IsRead = updatedBook.IsRead;

        // _context.Books.Update(existingBook);
        await _bookRepo.UpdateAsync(existingBook);
        if (await _bookRepo.SaveAsync())
        {
            return Ok(existingBook);
        }
        return StatusCode(500, "Internal Server Error");
    }

    // Add a book to an event
    [HttpPatch("addtoevent/{bookId}/{eventId}")]
    // [Authorize(Roles = "User")]
    public async Task<IActionResult> AddToEvent(string bookId, string eventId)
    {
        var book = await _bookRepo.FindByIdAsync(bookId);
        if (book is null) return NotFound($"Boken med ID {bookId} kunde inte hittas");

        var bookEvent = await _eventRepo.FindByIdAsync(eventId);
        if (bookEvent is null) return NotFound($"Eventet med ID {eventId} kunde inte hittas");

        // book.EventId = eventId;
        // bookEvent.Books.Clear();

        bookEvent.Books.Add(book);
        await _eventRepo.UpdateAsync(bookEvent);

        if (await _eventRepo.SaveAsync())
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

    // Add a book to a user's library
    [HttpPatch("addtolibrary/{bookId}/{userId}")]
    public async Task<IActionResult> AddToLibrary(string bookId, string userId)
    {
        // var book = await _context.Books.FindAsync(bookId);
        var book = await _bookRepo.FindByIdAsync(bookId);
        if (book is null) return NotFound($"Boken med ID {bookId} kunde inte hittas");

        // var user = await _context.Users.FindAsync(userId);
        var user = await _userRepo.FindByIdAsync(userId);
        if (user is null) return NotFound($"Användare med ID {userId} kunde inte hittas");

        // await user.Books.AddAsync(book);
        user.Books.Add(book);
        await _userRepo.UpdateAsync(user);

        if (await _userRepo.SaveAsync())
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

    // Remove a book
    [HttpDelete("{id}")]
    // [Authorize(Roles = "User")]
    public async Task<IActionResult> RemoveBook(string id)
    {
        var existingBook = await _bookRepo.FindByIdAsync(id);

        if (existingBook is null) return NotFound($"Bok ({id}) kunde inte hittas");

        await _bookRepo.DeleteAsync(existingBook);

        if (await _bookRepo.SaveAsync())
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }
}