
using Models;

namespace Services;
public interface IEventService
{
    Task<IList<Event>> ListAllAsync();
    Task<Event?> FindByIdAsync(string id);
    Task<bool> AddAsync(Event e);
    Task<bool> UpdateAsync(Event e);
    Task<bool> DeleteAsync(Event e);
    Task<bool> SaveAsync();
}