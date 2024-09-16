using BlogAPI.Data;
using BlogAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace BlogAPI.Repositories.Concrete;

public class UserRepository : IUserRepository
{
    
    private readonly UserManager<AppUser> _userManager;
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;

    public UserRepository(UserManager<AppUser> userManager,IMemoryCache memoryCache, IDistributedCache distributedCache)
    {
        
        _userManager = userManager;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
       
        return await _userManager.Users.AsNoTracking().Where(u => !u.IsDeleted && !u.IsBanned).ToListAsync();
    }

    public async Task<AppUser?> GetUserByIdAsync(string id)
    {
        string key = $"user-{id}";

        string? cachedMember = await _distributedCache.GetStringAsync(key);

        AppUser? appUser;
        if (string.IsNullOrEmpty(cachedMember))
        {
            appUser = await _userManager.FindByIdAsync(id);
            if (appUser is null)
            {
                return appUser;
            }

            await _distributedCache.SetStringAsync(key,JsonConvert.SerializeObject(appUser));
            return appUser;
        }

        appUser = JsonConvert.DeserializeObject<AppUser>(cachedMember);
        return appUser;
    }

    public async Task<bool> UpdateUserAsync(AppUser user)
    {
        var result= await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(AppUser user)
    {
        user.IsDeleted = true;
        var result= await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<AppUser> GetUserByEmailAsync(string email)
    {
        string key = $"user-{email}";
        
        string? cachedMember = await _distributedCache.GetStringAsync(key);

        AppUser? appUser;
        if (string.IsNullOrEmpty(cachedMember))
        {
            appUser = await _userManager.FindByEmailAsync(email);
            if (appUser is null)
            {
                return appUser;
            }

            await _distributedCache.SetStringAsync(key,JsonConvert.SerializeObject(appUser));
            return appUser;
        }

        appUser = JsonConvert.DeserializeObject<AppUser>(cachedMember);
        return appUser;
    }

    public async Task<AppUser> GetUserByUserNameAsync(string userName)
    {
        string key = $"user-{userName}";
        
        string? cachedMember = await _distributedCache.GetStringAsync(key);

        AppUser? appUser;
        if (string.IsNullOrEmpty(cachedMember))
        {
            appUser = await _userManager.FindByNameAsync(userName);
            if (appUser is null)
            {
                return appUser;
            }

            await _distributedCache.SetStringAsync(key,JsonConvert.SerializeObject(appUser));
            return appUser;
        }

        appUser = JsonConvert.DeserializeObject<AppUser>(cachedMember);
        return appUser;
    }
}
