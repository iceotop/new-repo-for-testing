using api.Data;
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
    private readonly BookCircleContext _context;

    public BookController(BookCircleContext context)
    {
        _context = context;
    }

    // Display all books
    [HttpGet]
    public IActionResult ListAllBooks()
    {
        var list = _context.Books.ToList();

        if (list is []) return NotFound($"Böcker kunde inte hittas");

        return Ok(list);
    }

    // Hämta enskild bok på ID
    [HttpGet("{id}")]
    // [Authorize(Roles = "User")]
    // [Authorize(Roles = "User, Admin")] - Om man vill lägga till flera roller
    public async Task<ActionResult> GetById(string id)
    {
        var result = await _context.Books.Select(b => new
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            PublicationYear = b.PublicationYear,
            Review = b.Review,
            IsRead = b.IsRead
        })
        .SingleOrDefaultAsync(b => b.Id == id);

        if (result is null) return BadRequest($"Boken med ID {id} kunde inte hittas");

        return Ok(result);
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

        await _context.Books.AddAsync(book);

        if (await _context.SaveChangesAsync() > 0)
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
        var existingBook = await _context.Books.FindAsync(id);

        if (existingBook is null) return NotFound($"Bok ({id}) finns inte i systemet");

        existingBook.Title = updatedBook.Title;
        existingBook.Author = updatedBook.Author;
        existingBook.PublicationYear = updatedBook.PublicationYear;
        existingBook.Review = updatedBook.Review;
        existingBook.IsRead = updatedBook.IsRead;

        _context.Books.Update(existingBook);
        if (await _context.SaveChangesAsync() > 0)
        {
            return Ok(existingBook);
        }
        return StatusCode(500, "Internal Server Error");
    }

    // Add a book to an event
    [HttpPatch("{bookId}/{eventId}")]
    // [Authorize(Roles = "User")]
    public async Task<IActionResult> AddToEvent(string bookId, string eventId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book is null) return NotFound($"Boken med ID {bookId} kunde inte hittas");

        book.EventId = eventId;
        _context.Books.Update(book);

        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }

    [HttpPatch("addtolibrary/{bookId}/{userId}")]
    public async Task<IActionResult> AddToLibrary(string bookId, string userId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book is null) return NotFound($"Boken med ID {bookId} kunde inte hittas");

        var user = await _context.Users.FindAsync(userId);
        if (user is null) return NotFound($"Användare med ID {userId} kunde inte hittas");

        user.Books.Add(book);
        _context.Users.Update(user);

        if (await _context.SaveChangesAsync() > 0)
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
        var existingBook = await _context.Books.FindAsync(id);

        if (existingBook is null) return NotFound($"Bok ({id}) kunde inte hittas");

        _context.Books.Remove(existingBook);
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }
}