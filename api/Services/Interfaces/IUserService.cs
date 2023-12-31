using Models;

namespace Services;
public interface IUserService
{
    Task<IList<UserModel>> ListAllAsync();
    Task<UserModel?> FindByIdAsync(string id);
    Task<UserModel?> FindByEmailAsync(string email);
    Task<bool> UpdateAsync(UserModel user);
    Task<bool> AddAsync(UserModel user);
    Task<bool> DeleteAsync(UserModel user);
    Task<bool> SaveAsync();
}