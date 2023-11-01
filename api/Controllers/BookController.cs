using System.Security.Claims;
using api.Data;
using api.Interfaces;
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
    public async Task<IActionResult> ListAllBooksAsync()
    {
        var list = await _bookRepo.ListAllAsync();

        if (list is []) return NotFound($"Böcker kunde inte hittas");

        return Ok(list);
    }

    // Hämta enskild bok på ID
    [HttpGet("{id}")]
    [Authorize(Roles = "User")]
    // [Authorize(Roles = "User, Admin")] - Om man vill lägga till flera roller
    public async Task<ActionResult> GetById(string id)
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
            ReadStatus = result.ReadStatus
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
            ReadStatus = model.ReadStatus
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
        existingBook.ReadStatus = updatedBook.ReadStatus;

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
        var user = await _userRepo.FindByEmailAsync(email);

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
        await _userRepo.UpdateAsync(user);
        

        // Save changes
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