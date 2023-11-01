using api.Data;
using api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace api.Repository;
public class UserRepository : IUserRepository
{
    private readonly BookCircleContext _context;
    public UserRepository(BookCircleContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(UserModel user)
    {
        try
        {
            await _context.Users.AddAsync(user);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<bool> DeleteAsync(UserModel user)
    {
        try
        {
            _context.Users.Remove(user);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<UserModel?> FindByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => u.Email == email)
            .Include(u => u.Books)
            .Include(u => u.Events)
            .SingleOrDefaultAsync(c => c.Email.Trim().ToLower() == email.Trim().ToLower());
    }

    public async Task<UserModel?> FindByIdAsync(string id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Include(u => u.Books)
            .Include(u => u.Events)
            .SingleOrDefaultAsync();
    }

    public async Task<IList<UserModel>> ListAllAsync()
    {
        return await _context.Users.ToListAsync();
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

    public Task<bool> UpdateAsync(UserModel user)
    {
        try
        {
            _context.Users.Update(user);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}