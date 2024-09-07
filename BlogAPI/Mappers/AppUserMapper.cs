using BlogAPI.DTOs;
using BlogAPI.Model;

namespace BlogAPI.Mappers;

public static class AppUserMapper
{
    // Map AppUser entity to AppUserGetDto
    public static AppUserGetDto ToDto(this AppUser appUser)
    {
        return new AppUserGetDto
        {
            Id = appUser.Id,
            Name = appUser.Name,
            MiddleName = appUser.MiddleName,
            FamilyName = appUser.FamilyName,
            UserName = appUser.UserName,
            Email = appUser.Email,
            PhoneNumber = appUser.PhoneNumber,
            NumOfPosts = appUser.NumOfPosts,
            Gender = appUser.Gender,
            BirthDate = appUser.BirthDate,
            ProfilePicturePath = appUser.ProfilePicturePath
        };
    }

    // Map AppUserCreateDto to AppUser entity
    public static AppUser ToEntity(this AppUserCreateDto appUserCreateDto)
    {
        return new AppUser
        {
            Name = appUserCreateDto.Name,
            MiddleName = appUserCreateDto.MiddleName,
            FamilyName = appUserCreateDto.FamilyName,
            UserName = appUserCreateDto.UserName,
            Email = appUserCreateDto.Email,
            PhoneNumber = appUserCreateDto.PhoneNumber,
            Gender = appUserCreateDto.Gender,
            BirthDate = appUserCreateDto.BirthDate,
            Password = appUserCreateDto.Password, // NotMapped, only for validation
            ConfirmPassword = appUserCreateDto.ConfirmPassword, // NotMapped, only for validation
            RegisterDate = DateTime.Now, // Set the register date when a new user is created
            IsDeleted = false, // Default to not deleted
            IsBanned = false // Default to not banned
        };
    }

    // Update an existing AppUser entity with AppUserCreateDto values
    public static void UpdateEntity(this AppUser appUser, AppUserCreateDto appUserCreateDto)
    {
        appUser.Name = appUserCreateDto.Name;
        appUser.MiddleName = appUserCreateDto.MiddleName;
        appUser.FamilyName = appUserCreateDto.FamilyName;
        appUser.UserName = appUserCreateDto.UserName;
        appUser.Email = appUserCreateDto.Email;
        appUser.PhoneNumber = appUserCreateDto.PhoneNumber;
        appUser.Gender = appUserCreateDto.Gender;
        appUser.BirthDate = appUserCreateDto.BirthDate;
    }
}
