using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;

using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Model;
using BlogAPI.Repositories;
using BlogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.Services.Concrete;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, IMapper mapper, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<ServiceResult<IEnumerable<AppUserGetDto>>> GetUsersAsync()
    {
        var users = await _userRepository.GetUsersAsync();
        if (users == null)
        {
            return ServiceResult<IEnumerable<AppUserGetDto>>.FailureResult("Users not found.");
        }

        var result = _mapper.Map<IEnumerable<AppUserGetDto>>(users);
        return ServiceResult<IEnumerable<AppUserGetDto>>.SuccessResult(result);
    }

    public async Task<ServiceResult<AppUserGetDto>> GetAppUserAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return ServiceResult<AppUserGetDto>.FailureResult("User not found.");
        }

        var result = _mapper.Map<AppUserGetDto>(user);
        return ServiceResult<AppUserGetDto>.SuccessResult(result);
    }

    public async Task<ServiceResult<AppUserGetDto>> PutAppUserAsync(string id, AppUserCreateDto appUserDto)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return ServiceResult<AppUserGetDto>.FailureResult("User not found.");
        }

        _mapper.Map(appUserDto, user);
        var isUpdated = await _userRepository.UpdateUserAsync(user);
        if (isUpdated)
        {
            var result = _mapper.Map<AppUserGetDto>(user);
            return ServiceResult<AppUserGetDto>.SuccessResult(result);
        }

        return ServiceResult<AppUserGetDto>.FailureResult("Failed to update user.");
    }

    public async Task<ServiceResult<AppUserGetDto>> PostAppUserAsync(AppUserCreateDto userDto)
    {
        var appUser = _mapper.Map<AppUser>(userDto);
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
            var message = new Message(new List<string> { userDto.Email }, "Email Confirmation Token", callback);
            await _emailSender.SendEmailAsync(message);

            var mappedUser = _mapper.Map<AppUserGetDto>(appUser);
            return ServiceResult<AppUserGetDto>.SuccessResult(mappedUser);
        }

        return ServiceResult<AppUserGetDto>.FailureResult("Failed to create user.");
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<bool> BanUserAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.IsBanned = true;
        return await _userRepository.UpdateUserAsync(user);
    }

    public async Task<bool> DeleteAppUserAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        return await _userRepository.DeleteUserAsync(user);
    }

    public async Task<ServiceResult<string>> LoginAsync(string userName, string password)
    {
        var appUser = await _userRepository.GetUserByUserNameAsync(userName);
        if (appUser == null || appUser.IsDeleted)
        {
            return ServiceResult<string>.FailureResult("Invalid username or password.");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(appUser, password, false);
        if (signInResult.Succeeded)
        {
            var token = await CreateJwtToken(appUser);
            var toStringToken = new JwtSecurityTokenHandler().WriteToken(token);
            return ServiceResult<string>.SuccessResult(toStringToken);
        }

        return ServiceResult<string>.FailureResult("Invalid username or password.");
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
            return ServiceResult<string>.FailureResult("Invalid image format.");
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
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult<string>.FailureResult("User not found.");
        }

        user.ProfilePicturePath = Path.Combine("images", fileName);
        await _userRepository.UpdateUserAsync(user);

        return ServiceResult<string>.SuccessResult(user.ProfilePicturePath);
    }

    public async Task<ServiceResult<string>> UpdateProfilePictureAsync(string userId, IFormFile file)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult<string>.FailureResult("User not found.");
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

        return await UploadProfilePictureAsync(userId, file);
    }

    public async Task<ServiceResult<bool>> DeleteProfilePictureAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult<bool>.FailureResult("User not found.");
        }

        if (!string.IsNullOrEmpty(user.ProfilePicturePath))
        {
            var filePath = Path.Combine("wwwroot", user.ProfilePicturePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            user.ProfilePicturePath = null;
            await _userRepository.UpdateUserAsync(user);

            return ServiceResult<bool>.SuccessResult(true);
        }

        return ServiceResult<bool>.FailureResult("No profile picture to delete.");
    }

    public async Task<bool> ForgetPasswordAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
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
        var user = await _userRepository.GetUserByEmailAsync(resetPasswordForm.Email);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, resetPasswordForm.Token, resetPasswordForm.Password);
        return result.Succeeded;
    }

    private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: signingCredentials);

        return token;
    }
}

