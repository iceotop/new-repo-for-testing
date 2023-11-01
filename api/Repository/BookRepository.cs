using api.Data;
using api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;

namespace api.Repository;
public class BookRepository : IBookRepository
{
    private readonly BookCircleContext _context;
    public BookRepository(BookCircleContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(Book book)
    {
        try
        {
            await _context.Books.AddAsync(book);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Book?> FindByIdAsync(string id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task<IList<Book>> ListAllAsync()
    {
        return await _context.Books.ToListAsync();
    }

    public Task<bool> UpdateAsync(Book book)
    {
        try
        {
            _context.Books.Update(book);
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
            if (await _context.SaveChangesAsync() > 0) return true;
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
            _context.Books.Remove(book);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}