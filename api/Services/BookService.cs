using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;

namespace Services;

public class BookService : BaseService, IBookService
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

    // public async Task<Book?> FindByIdAsync(string id)
    // {
    //     return await _databaseConnection.Books.FindAsync(id);
    // }

    public async Task<Book?> FindByIdAsync(string id)
    {
        var cacheKey = $"book:{id}";

        var cachedBook = redisDatabase.StringGet(cacheKey);

        if (cachedBook.HasValue)
        {
            return JsonSerializer.Deserialize<Book>(cachedBook);
        }

        // simulera långsam laddning för att dema cachning
        await Task.Delay(2000);

        // var database = new DatabaseConnection();

        var book = await _databaseConnection.Books.FirstOrDefaultAsync(c => c.Id == id);

        if (book is not null)
        {
            redisDatabase.StringSet(cacheKey, JsonSerializer.Serialize(book), TimeSpan.FromHours(1));
        }

        return book;

        // return await _databaseConnection.Books.FindAsync(id);
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