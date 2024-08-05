using BlogAPI.DTOs;
using BlogAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<AppUserGetDto>?> GetUsersAsync();
    Task<AppUserGetDto?> GetAppUserAsync(string id);
    Task<AppUserGetDto?> PutAppUserAsync(string id, AppUserCreateDto appUser);
    Task<AppUserGetDto?> PostAppUserAsync(AppUserCreateDto appUser);
    Task<bool> ConfirmEmailAsync(string email, string token);
    Task<bool> BanUserAsync(string id);
    Task<bool> DeleteAppUserAsync(string id);
    Task<bool> LoginAsync(string userName, string password);
    Task<bool> LogoutAsync();
    Task<bool> ForgetPasswordAsync(string email);
    Task<ResetPasswordForm> ResetPassswordAbc(string email, string token);
    Task<bool> ResetPasswordAsync(ResetPasswordForm resetPasswordForm);

}