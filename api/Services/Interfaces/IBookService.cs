using Models;

namespace Services;
public interface IBookService
{
    Task<IList<Book>> ListAllAsync();
    Task<Book?> FindByIdAsync(string id);
    Task<bool> AddAsync(Book book);
    Task<bool> UpdateAsync(Book book);
    Task<bool> DeleteAsync(Book book);
    Task<bool> SaveAsync();
}