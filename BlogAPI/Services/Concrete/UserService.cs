using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class UserService:IUserService
{
    
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ApplicationContext _context;
    private readonly IEmailSender _emailSender;

    public UserService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,ApplicationContext context,IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _emailSender = emailSender;
    }
    public async Task<IEnumerable<AppUserGetDto>?> GetUsersAsync()
    {
        if (_context.Users == null)
        {
            return null;
        }
        var users =await _context.Users.Where(u=>u.IsDeleted==false&&u.IsBanned==false).ToListAsync();
        if (users == null)
        {
            return null;
        }
        return users.Select(user => new AppUserGetDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            MiddleName = user.Name,
            FamilyName = user.FamilyName,
            Email = user.Email,
            
            NumOfPosts = user.NumOfPosts,
            Gender = user.Gender,
            BirthDate = user.BirthDate
                    
        });
    }

    public async  Task<AppUserGetDto?> GetAppUserAsync(string id)
    {
        if (_context.Users == null)
        {
            return null;
        }
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        return new AppUserGetDto
        {
            Id = user.Id,
            Name = user.Name,
            MiddleName = user.Name,
            FamilyName = user.FamilyName,
            NumOfPosts = user.NumOfPosts,
            Gender = user.Gender,
            BirthDate = user.BirthDate,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email
        };
    }

    public async Task<AppUserGetDto?> PutAppUserAsync(string id, AppUserCreateDto appUser)
    {
        if(_context.Users==null)
        {
            return null;
        }
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null;
        }
        user.BirthDate = appUser.BirthDate;
        user.Email = appUser.Email;
        user.Name = appUser.Name;
        user.Gender = appUser.Gender;
        user.MiddleName = appUser.MiddleName;
        user.FamilyName = appUser.FamilyName;
        user.PhoneNumber = appUser.PhoneNumber;
        user.UserName = appUser.UserName;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return new AppUserGetDto
            {
                Id = user.Id,
                Name = user.Name,
                MiddleName = user.Name,
                FamilyName = user.FamilyName,
                NumOfPosts = user.NumOfPosts,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };
        }

        return null;
    }

    public async Task<AppUserGetDto?> PostAppUserAsync(AppUserCreateDto user)
    {
        if (_context.Users == null)
        {
            return null;
        }
        AppUser appUser = new AppUser
        {
            UserName = user.UserName,
            Name = user.Name,
            MiddleName = user.Name,
            FamilyName = user.FamilyName,
            
            Gender = user.Gender,
            BirthDate = user.BirthDate,
           
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Password = user.Password
        };
        var result = await _userManager.CreateAsync(appUser, appUser.Password);
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        var param = new Dictionary<string, string?>
        {
            {"token",token},
            {"email",appUser.Email}
        };
        var callback = QueryHelpers.AddQueryString("http://localhost:5231/api/AppUsers/emailConfirmation", param);
        IEnumerable<string> emails = new List<string> { user.Email };

        var message = new Message(emails, "Email Confirmation Token", callback);
        await _emailSender.SendEmailAsync(message);
        if (result.Succeeded)
        {
            return new AppUserGetDto
            {
                Id = appUser.Id,
                Name = appUser.Name,
                MiddleName = appUser.Name,
                FamilyName = appUser.FamilyName,
                NumOfPosts = appUser.NumOfPosts,
                Gender = appUser.Gender,
                BirthDate = appUser.BirthDate,
                UserName = appUser.UserName,
                PhoneNumber = appUser.PhoneNumber,
                Email = appUser.Email
            };
        }

        return null;
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return false;
        var confirmResult =await _userManager.ConfirmEmailAsync(user, token);
        if (!confirmResult.Succeeded)
        {
            return false;
        }
        return true;
    }
    public async Task<bool> BanUserAsync(string id)
    {
        if (_userManager.Users == null)
        {
            return false;
        }

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
        if (_context.Users == null)
        {
            return false;
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user==null)
        {
            return false;
        }

        user.IsDeleted = true;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> LoginAsync(string userName, string password)
    {
        var appUser = await _userManager.FindByNameAsync(userName);
        if (appUser == null)
        {
            return false;
        }
        var signInResult = await _signInManager.PasswordSignInAsync(appUser,password,false,false);
        return signInResult.Succeeded;
    }

    public async Task<bool> LogoutAsync()
    {
         await _signInManager.SignOutAsync();
         return true;
    }

    public async Task<bool> ForgetPasswordAsync(string email)
    {
        AppUser user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var token= _userManager.GeneratePasswordResetTokenAsync(user).Result;
            var param = new Dictionary<string, string?>
            {
                {"token",token},
                {"email",user.Email}
            };
            var callback = QueryHelpers.AddQueryString("http://localhost:5231/api/AppUsers/ResetPassword", param);
            var message = new Message(new List<string> { user.Email }, "Reset password", callback);
            await _emailSender.SendEmailAsync(message);
            return true;
        }

        return false;
    }

    public async Task<ResetPasswordForm> ResetPassswordAbc(string email,string token)
    {
        var model = new ResetPasswordForm { Token = token,Email = email};
        return model;
    }

    public async  Task<bool> ResetPasswordAsync(ResetPasswordForm resetPasswordForm)
    {
        AppUser user = await _userManager.FindByEmailAsync(resetPasswordForm.Email);
        if (user == null)
        {
            return false;
        }
        var result = await _userManager.ResetPasswordAsync(user, resetPasswordForm.Token, resetPasswordForm.Password);
        return result.Succeeded;
    }
}