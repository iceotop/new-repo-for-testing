using System.Security.Claims;
using api.Data;
using api.Models;
using api.Models.DTOs;
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
    [Authorize(Roles = "User")]
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
            IsRead = b.ReadStatus
        })
        .SingleOrDefaultAsync(b => b.Id == id);

        if (result is null) return BadRequest($"Boken med ID {id} kunde inte hittas");

        return Ok(result);
    }

    // Add a new book
    [HttpPost()]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> Create(BookBaseViewModel model)
    {
        var book = new Book
        {
            Id = Guid.NewGuid().ToString(),
            Title = model.Title,
            Author = model.Author,
            PublicationYear = model.PublicationYear,
            Review = model.Review,
            ReadStatus = model.ReadStatus
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
    [Authorize(Roles = "User")]
    public async Task<IActionResult> EditBook(string id, [FromBody] Book updatedBook)
    {
        var existingBook = await _context.Books.FindAsync(id);

        if (existingBook is null) return NotFound($"Bok ({id}) finns inte i systemet");

        existingBook.Title = updatedBook.Title;
        existingBook.Author = updatedBook.Author;
        existingBook.PublicationYear = updatedBook.PublicationYear;
        existingBook.Review = updatedBook.Review;
        existingBook.ReadStatus = updatedBook.ReadStatus;

        _context.Books.Update(existingBook);
        if (await _context.SaveChangesAsync() > 0)
        {
            return Ok(existingBook);
        }
        return StatusCode(500, "Internal Server Error");
    }

    // Add a book to an event
    [HttpPatch("{bookId}/{eventId}")]
    [Authorize(Roles = "User")]
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

    [HttpPatch("addtolibrary")]
    public async Task<IActionResult> AddToLibrary([FromBody] AddBookDto book)
    {
        // Extract the email from the token in the header
        var emailClaim = User.FindFirst(claim => claim.Type == ClaimTypes.Email);
        if (emailClaim == null)
        {
            return Unauthorized("No email claim found in token");
        }

        string email = emailClaim.Value;

        // Fetch the user from the database
        var user = await _context.Users
            .Where(u => u.Email == email)
            .Include(u => u.Books)
            .SingleOrDefaultAsync();

        if (user == null)
        {
            return NotFound($"User with email {email} could not be found");
        }

        // Convert the incoming book DTO to your Book model
        var newBook = new Book
        {
            Id = Guid.NewGuid().ToString(),
            Title = book.Title,
            Author = book.Author,
            PublicationYear = book.PublicationYear,
            Review = book.Review,
            ReadStatus = book.ReadStatus,
            ImageUrl = book.ImageUrl
        };

        // Add the new book to the user's Books collection
        user.Books.Add(newBook);

        // Update the user in the database
        _context.Users.Update(user);
        

        // Save changes
        if (await _context.SaveChangesAsync() > 0)
        {
            return NoContent();
        }

        return StatusCode(500, "Internal Server Error");
    }


    // Remove a book
    [HttpDelete("{id}")]
    [Authorize(Roles = "User")]
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