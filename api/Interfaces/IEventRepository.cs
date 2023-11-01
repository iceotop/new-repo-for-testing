
using Models;

namespace api.Interfaces;
public interface IEventRepository
{
    Task<IList<Event>> ListAllAsync();
    Task<Event?> FindByIdAsync(string id);
    Task<bool> AddAsync(Event e);
    Task<bool> UpdateAsync(Event e);
    Task<bool> DeleteAsync(Event e);
    Task<bool> SaveAsync();
}