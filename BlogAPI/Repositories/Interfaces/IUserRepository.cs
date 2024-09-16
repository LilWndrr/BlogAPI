using BlogAPI.Model;

namespace BlogAPI.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(string id);
    Task<bool> UpdateUserAsync(AppUser user);
    Task<bool> DeleteUserAsync(AppUser user);
    Task<AppUser> GetUserByEmailAsync(string email);
    Task<AppUser> GetUserByUserNameAsync(string userName);
}
