using api.Models;

namespace api.Interfaces;
public interface IUserRepository
{
    Task<IList<UserModel>> ListAllAsync();
    Task<UserModel?> FindByIdAsync(string id);
    Task<UserModel?> FindByEmailAsync(string email);
    // Task<bool> UpdateAsync(UserModel user); - Kanske behövs sen?
    Task<bool> DeleteAsync(UserModel user);
    Task<bool> SaveAsync();
}