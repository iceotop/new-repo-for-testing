using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;

namespace Services;

public class EventService
{
    private readonly DatabaseConnection _databaseConnection;
    public EventService(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<List<Event>> Get()
    {
        var events = await _databaseConnection.Events.ToListAsync();
        return events;
    }
}