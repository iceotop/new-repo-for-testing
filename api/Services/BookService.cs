using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;

namespace Services;

public class BookService
{
    private readonly DatabaseConnection _databaseConnection;
    public BookService(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }
    public async Task<bool> AddAsync(Book book)
    {
        try
        {
            await _databaseConnection.Books.AddAsync(book);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Book?> FindByIdAsync(string id)
    {
        return await _databaseConnection.Books.FindAsync(id);
    }

    public async Task<IList<Book>> ListAllAsync()
    {
        return await _databaseConnection.Books.ToListAsync();
    }

    public Task<bool> UpdateAsync(Book book)
    {
        try
        {
            _databaseConnection.Books.Update(book);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<bool> SaveAsync()
    {
        try
        {
            if (await _databaseConnection.SaveChangesAsync() > 0) return true;
            return false;
        }
        catch
        {
            return false;
        }
    }

    public Task<bool> DeleteAsync(Book book)
    {
        try
        {
            _databaseConnection.Books.Remove(book);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}