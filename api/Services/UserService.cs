using api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseConnection _databaseConnection;
        public UserService(DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }
        public async Task<bool> AddAsync(UserModel user)
        {
            try
            {
                await _databaseConnection.Users.AddAsync(user);
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
                _databaseConnection.Users.Remove(user);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<UserModel?> FindByEmailAsync(string email)
        {
            return await _databaseConnection.Users
                .Where(u => u.Email == email)
                .Include(u => u.Books)
                .Include(u => u.Events)
                .SingleOrDefaultAsync(c => c.Email.Trim().ToLower() == email.Trim().ToLower());
        }

        public async Task<UserModel?> FindByIdAsync(string id)
        {
            return await _databaseConnection.Users
                .Where(u => u.Id == id)
                .Include(u => u.Books)
                .Include(u => u.Events)
                .SingleOrDefaultAsync();
        }

        public async Task<IList<UserModel>> ListAllAsync()
        {
            return await _databaseConnection.Users.ToListAsync();
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

        public Task<bool> UpdateAsync(UserModel user)
        {
            try
            {
                _databaseConnection.Users.Update(user);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}