using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Services.Interfaces;

public interface IUserService
{
    Task<ServiceResult<IEnumerable<AppUserGetDto>>>  GetUsersAsync();
    Task<ServiceResult<AppUserGetDto>> GetAppUserAsync(string id);
    Task<ServiceResult<AppUserGetDto>> PutAppUserAsync(string id, AppUserCreateDto appUser);
    Task<ServiceResult<AppUserGetDto>> PostAppUserAsync(AppUserCreateDto appUser);
    Task<bool> ConfirmEmailAsync(string email, string token);
    Task<bool> BanUserAsync(string id);
    Task<bool> DeleteAppUserAsync(string id);
    Task<ServiceResult<string>> LoginAsync(string userName, string password);
    Task<bool> LogoutAsync();
    Task<bool> ForgetPasswordAsync(string email);
    Task<ServiceResult<string>> UploadProfilePictureAsync(string userId, IFormFile file);
    Task<ServiceResult<bool>> DeleteProfilePictureAsync(string userId);
    Task<ServiceResult<string>> UpdateProfilePictureAsync(string userId, IFormFile file);
    Task<ResetPasswordForm> ResetPasswordAbc(string email, string token);
    Task<bool> ResetPasswordAsync(ResetPasswordForm resetPasswordForm);

}