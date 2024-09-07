using AutoMapper;
using BlogAPI.DTOs;
using BlogAPI.Model;

namespace BlogAPI.HelperServices;

public class AutomapperProfile:Profile
{
    public AutomapperProfile()
    {
        CreateMap<AppUser, AppUserGetDto>();
        CreateMap<AppUserCreateDto, AppUser>();
       
        
    }
}   