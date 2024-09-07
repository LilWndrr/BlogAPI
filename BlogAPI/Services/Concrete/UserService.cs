using AutoMapper;
using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IMapper _mapper;

    public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationContext context, IEmailSender emailSender, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _emailSender = emailSender;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<AppUserGetDto>>> GetUsersAsync()
    {
        if (_context.Users == null)
        {
            return ServiceResult<IEnumerable<AppUserGetDto>>.FailureResult("Users context is not available.");
        }

        var users = await _context.Users.Where(u => !u.IsDeleted && !u.IsBanned).ToListAsync();
        var result = _mapper.Map<IEnumerable<AppUserGetDto>>(users);
        return ServiceResult<IEnumerable<AppUserGetDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<AppUserGetDto>> GetAppUserAsync(string id)
    {
        if (_context.Users == null)
        {
            return ServiceResult<AppUserGetDto>.FailureResult("Users context is not available.");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult<AppUserGetDto>.FailureResult("User not found.");
        }

        var result = _mapper.Map<AppUserGetDto>(user);
        return ServiceResult<AppUserGetDto>.SuccessResult(result);
    }

    public async Task<ServiceResult<AppUserGetDto>> PutAppUserAsync(string id, AppUserCreateDto appUser)
    {
        if (_context.Users == null)
        {
            return ServiceResult<AppUserGetDto>.FailureResult("Users context is not available.");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult<AppUserGetDto>.FailureResult("User not found.");
        }

        _mapper.Map(appUser, user);
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            var mappedUser = _mapper.Map<AppUserGetDto>(user);
            return ServiceResult<AppUserGetDto>.SuccessResult(mappedUser);
        }

        return ServiceResult<AppUserGetDto>.FailureResult("Failed to update user.");
    }

    public async Task<ServiceResult<AppUserGetDto>> PostAppUserAsync(AppUserCreateDto user)
    {
        if (_context.Users == null)
        {
            return ServiceResult<AppUserGetDto>.FailureResult("Users context is not available.");
        }

        var appUser = _mapper.Map<AppUser>(user);
        var result = await _userManager.CreateAsync(appUser, appUser.Password);
        if (result.Succeeded)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            var param = new Dictionary<string, string?>
            {
                {"token", token},
                {"email", appUser.Email}
            };
            var callback = QueryHelpers.AddQueryString("http://localhost:5231/api/AppUsers/emailConfirmation", param);
            var message = new Message(new List<string> { user.Email }, "Email Confirmation Token", callback);
            await _emailSender.SendEmailAsync(message);

            var mappedUser = _mapper.Map<AppUserGetDto>(appUser);
            return ServiceResult<AppUserGetDto>.SuccessResult(mappedUser);
        }

        return ServiceResult<AppUserGetDto>.FailureResult("Failed to create user.");
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<bool> BanUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.IsBanned = true;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAppUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.IsDeleted = true;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> LoginAsync(string userName, string password)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(userName, password, false, false);
        return signInResult.Succeeded;
    }

    public async Task<bool> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return true;
    }

    public async Task<ServiceResult<string>> UploadProfilePictureAsync(string userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return ServiceResult<string>.FailureResult("Invalid image format.") ;
        }

        // Define the folder path
        var folderPath = Path.Combine("wwwroot", "images");
        Directory.CreateDirectory(folderPath); // Ensure the directory exists
        
        // Generate a unique filename
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(folderPath, fileName);

        // Save the file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Update the user's profile picture path
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult<string>.FailureResult("User was not found") ;
        }

        user.ProfilePicturePath = Path.Combine("images", fileName);
        await _context.SaveChangesAsync();

        return ServiceResult<string>.SuccessResult(user.ProfilePicturePath); 
    }
    // Update profile picture logic
    public async Task<ServiceResult<string>> UpdateProfilePictureAsync(string userId, IFormFile file)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return ServiceResult<string>.FailureResult("User not found");
        }

        // Delete the current image if it exists
        if (!string.IsNullOrEmpty(user.ProfilePicturePath))
        {
            var oldImagePath = Path.Combine("wwwroot", user.ProfilePicturePath);
            if (File.Exists(oldImagePath))
            {
                File.Delete(oldImagePath);
            }
        }

        // Now upload the new image (reusing upload logic)
        return await UploadProfilePictureAsync(userId, file);
    }
        
    public async Task<ServiceResult<bool>> DeleteProfilePictureAsync(string  userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return  ServiceResult<bool>.FailureResult("User not found") ;
        }

        // Delete the image file from the file system
        if (!string.IsNullOrEmpty(user.ProfilePicturePath))
        {
            var filePath = Path.Combine("wwwroot", user.ProfilePicturePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Update the user to remove the profile picture reference
            user.ProfilePicturePath = null;
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true);
        }

        return ServiceResult<bool>.FailureResult("No profile picture to delete");
    }
    public async Task<bool> ForgetPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string?>
            {
                {"token", token},
                {"email", user.Email}
            };
            var callback = QueryHelpers.AddQueryString("http://localhost:5231/api/AppUsers/ResetPassword", param);
            var message = new Message(new List<string> { user.Email }, "Reset password", callback);
            await _emailSender.SendEmailAsync(message);
            return true;
        }

        return false;
    }



    public Task<ResetPasswordForm> ResetPasswordAbc(string email, string token)
    {
        return Task.FromResult(new ResetPasswordForm { Token = token, Email = email });
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordForm resetPasswordForm)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordForm.Email);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, resetPasswordForm.Token, resetPasswordForm.Password);
        return result.Succeeded;
    }
}
