using api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;

namespace Services;

public class EventService : IEventService
{
    private readonly DatabaseConnection _databaseConnection;
    public EventService(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public Task<bool> AddAsync(Event e)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Event e)
    {
        throw new NotImplementedException();
    }

    public Task<Event?> FindByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Event>> Get()
    {
        var events = await _databaseConnection.Events.ToListAsync();
        return events;
    }

    public Task<IList<Event>> ListAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Event e)
    {
        throw new NotImplementedException();
    }
}