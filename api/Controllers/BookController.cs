using api.Interfaces;
using api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace api.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepo;
    private readonly IUserRepository _userRepo;
    private readonly IEventRepository _eventRepo;

    public BookController(IBookRepository bookRepo, IUserRepository userRepo, IEventRepository eventRepo)
    {
        _eventRepo = eventRepo;
        _userRepo = userRepo;
        _bookRepo = bookRepo;
    }

    // Display all books
    [HttpGet]
    public async Task<IActionResult> ListAllBooks()
    {
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

        if (result == null)
        {
            return NotFound($"Boken med ID {id} kunde inte hittas");
        }

        var book = new
        {
            Id = result.Id,
            Title = result.Title,
            Author = result.Author,
            PublicationYear = result.PublicationYear,
            Review = result.Review,
            IsRead = result.IsRead
        };

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
        var existingBook = await _bookRepo.FindByIdAsync(id);

        if (existingBook is null) return NotFound($"Bok ({id}) finns inte i systemet");

        existingBook.Title = updatedBook.Title;
        existingBook.Author = updatedBook.Author;
        existingBook.PublicationYear = updatedBook.PublicationYear;
        existingBook.Review = updatedBook.Review;
        existingBook.IsRead = updatedBook.IsRead;

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
        var book = await _bookRepo.FindByIdAsync(bookId);
        if (book is null) return NotFound($"Boken med ID {bookId} kunde inte hittas");

        var user = await _userRepo.FindByIdAsync(userId);
        if (user is null) return NotFound($"Användare med ID {userId} kunde inte hittas");

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