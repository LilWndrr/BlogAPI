using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogAPI.DTOs;
using BlogAPI.Services.Interfaces;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
[ApiController]
public class AppUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AppUsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/AppUsers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUserGetDto>>> GetUsers()
    {
        var result = await _userService.GetUsersAsync();
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Data);
    }

    // GET: api/AppUsers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AppUserGetDto>> GetAppUser(string id)
    {
        var result = await _userService.GetAppUserAsync(id);
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Data);
    }

    // PUT: api/AppUsers/5
    [HttpPut("{id}")]
    public async Task<ActionResult<AppUserGetDto>> PutAppUser(string id, [FromBody] AppUserCreateDto appUser)
    {
        var result = await _userService.PutAppUserAsync(id, appUser);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Data);
    }

    // POST: api/AppUsers
    [HttpPost]
    public async Task<ActionResult<AppUserGetDto>> PostAppUser([FromBody] AppUserCreateDto appUser)
    {
        var result = await _userService.PostAppUserAsync(appUser);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Data);
    }

    [HttpGet("emailConfirmation")]
    public async Task<ActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token)
    {
        var result = await _userService.ConfirmEmailAsync(email, token);
        if (!result)
        {
            return BadRequest("Email confirmation failed.");
        }
        return Ok();
    }

    [HttpPost("banUser")]
    public async Task<ActionResult> BanUser([FromQuery] string id)
    {
        var result = await _userService.BanUserAsync(id);   
        if (!result)
        {
            return NotFound("User not found.");
        }
        return Ok();
    }

    // DELETE: api/AppUsers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppUser(string id)
    {
        var result = await _userService.DeleteAppUserAsync(id);
        if (!result)
        {
            return NotFound("User not found.");
        }
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromForm] string userName, [FromForm] string password)
    {
        var result = await _userService.LoginAsync(userName, password);
        if (!result.Success)
        {
            return Unauthorized("Invalid username or password.");
        }
        return Ok(result.Data);
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task<ActionResult> Logout()
    {
        await _userService.LogoutAsync();
        return Ok("Logout successful.");
    }
    [HttpPost("UploadProfilePicture")]
    
    [Authorize]
    public async  Task<ActionResult> UploadProfilePictureAsync( IFormFile file)
    {
        var userId = User.FindFirst("uid")?.Value;
        var result = await _userService.UploadProfilePictureAsync(userId, file);

        if (result.Success)
        {
            return Ok("Image uploaded successfully");
        }
        
        return BadRequest("Smth went wrong");
    }

    [HttpPut("UpdateProfilePicture/{userId}")]
    public async Task<IActionResult> UpdateProfilePicture(string userId,  IFormFile file)
    {
        var result = await _userService.UpdateProfilePictureAsync(userId, file);
    
        if (result.Success)
        {
            return Ok("Profile picture updated successfully");
        }

        return BadRequest("Smth went wrong");
    }
    [HttpDelete("DeleteProfilePicture/{userId}")]
    public async Task<IActionResult> DeleteProfilePicture(string userId)
    {
        var result = await _userService.DeleteProfilePictureAsync(userId);

        if (result.Success)
        {
            return Ok("Profile picture deleted successfully");
        }

        return BadRequest("Smth went wrong");
    }

    [HttpPost("forgetPassword")]
    public async Task<ActionResult> ForgetPassword([FromBody] string email)
    {
        var result = await _userService.ForgetPasswordAsync(email);
        if (!result)
        {
            return BadRequest("User not found.");
        }
        return Ok("Email sent for password reset.");
    }

    [HttpGet("resetPassword")]
    public async Task<ActionResult<ResetPasswordForm>> ResetPasswordAbc([FromQuery] string email, [FromQuery] string token)
    {
        var result = await _userService.ResetPasswordAbc(email, token);
        if (result == null)
        {
            return BadRequest("Invalid email or token.");
        }
        return Ok(result);
    }

    [HttpPost("resetPassword")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordForm resetPasswordForm)
    {
        var result = await _userService.ResetPasswordAsync(resetPasswordForm);
        if (!result)
        {
            return BadRequest("Password reset failed.");
        }
        return Ok("Password reset successful.");
    }
}

}
