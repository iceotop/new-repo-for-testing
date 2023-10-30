using api.Models;

namespace api.Interfaces;
public interface IBookRepository
{
    Task<IList<Book>> ListAllAsync();
    Task<Book?> FindByIdAsync(string id);
    Task<bool> AddAsync(Book book);
    Task<bool> UpdateAsync(Book book);
    Task<bool> AddBookToEventAsync(Book book);
    Task<bool> AddBookToLibraryAsync(Book book);
    Task<bool> DeleteAsync(Book book);
    Task<bool> SaveAsync();
}