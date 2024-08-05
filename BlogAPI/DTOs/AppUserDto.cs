using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTOs;

public class AppUserGetDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }

    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public int NumOfPosts { get; set; }

   
        
    public bool Gender { get; set; }
    public DateTime BirthDate { get; set; }
}

public class AppUserCreateDto
{
    
    public string Name { get; set; } = "";
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }

    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";

    public string PhoneNumber { get; set; } = "";
    public bool Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public string? UserUri { get; set; }
    
    public string? Password { get; set; }
    [Compare("Password")]
    public string? ConfirmPassword { get; set; }
}