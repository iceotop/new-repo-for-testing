using api.Data;
using api.Models;
using api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    // Hämta enskild bok på ID
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
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

    // Lägg till ny bok
    [HttpPost()]
    public async Task<ActionResult> Create(BookBaseViewModel model)
    {
        var book = new Book
        {
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
}