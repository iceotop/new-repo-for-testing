using api.Models;

namespace api.Interfaces;
public interface IEventRepository
{
    Task<IList<Event>> ListAllAsync();
    Task<Event?> FindByIdAsync(string id);
    Task<bool> AddAsync(Event e);
    Task<bool> UpdateAsync(Event e);
    Task<bool> DeleteAsync(Event e);
    Task<Event?> DetailsAsync(string id);
    Task<bool> UpdatePartialAsync(Event e);
    Task<bool> AddBookToEventAsync(Event e);
    Task<bool> AddBookToLibraryAsync(Event e);
    Task<bool> SaveAsync();
}