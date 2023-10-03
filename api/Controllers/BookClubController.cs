using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookClubController : ControllerBase
{
    public BookCircleContext _context;
    public BookClubController(BookCircleContext Context)
    {
        _context = Context;

    }

    [HttpGet]
    public async Task<ActionResult<List<Event>>> Get()
    {
        return Ok(await _context.Events.ToListAsync());
    }

    [HttpGet("{eventId}")]
    public async Task<ActionResult<Event>> Get(int eventId)
    {
        var bookEvent = await _context.Events.FindAsync(eventId);
        if (bookEvent == null)
            return BadRequest("Event not found.");
            return Ok(bookEvent);
    }

    [HttpPost]
    public async Task<ActionResult<List<Event>>> Add(Event newEvent)
    {
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        return Ok(await _context.Events.ToListAsync());
    }

    [HttpPut]
    public async Task<ActionResult<List<Event>>> Update(Event request)
    {
        var bookEvent = await _context.Events.FindAsync(request.EventId);
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

    [HttpDelete("{eventId}")]
    public async Task<ActionResult<List<Event>>> Delete(int eventId)
    {
        var bookEvent = await _context.Events.FindAsync(eventId);
        if (bookEvent == null)
            return BadRequest("Event not found.");

        _context.Events.Remove(bookEvent);

        await _context.SaveChangesAsync();
        return Ok(await _context.Events.ToListAsync());
    }

}