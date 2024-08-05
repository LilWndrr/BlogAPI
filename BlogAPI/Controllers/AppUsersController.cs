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
            var users = await _userService.GetUsersAsync();
            if (users == null)
            {
                return NotFound();
            }
            return  Ok(users);
        }
        

        // GET: api/AppUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUserGetDto>> GetAppUser(string id)
        {
          
            var appUser = await _userService.GetAppUserAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }
           

            return Ok(appUser);
        }

        // PUT: api/AppUsers/5
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ActionResult<AppUserGetDto>> PutAppUser(string id, [FromBody] AppUserCreateDto appUser)
        {
            var user = await _userService.PutAppUserAsync(id, appUser);
            if (user == null)
            {
                return BadRequest();
            }
            return Ok(user);
        }

        // POST: api/AppUsers
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async  Task<ActionResult<AppUserGetDto>> PostAppUser([FromBody] AppUserCreateDto appUser)
        {

            var user = await _userService.PostAppUserAsync(appUser);
            if (user == null)
            {
                return BadRequest();
            }
            

            return Ok(user);
        }

        [HttpGet("emailConfirmation")]
        public async Task<ActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token)
        {
            var result = await _userService.ConfirmEmailAsync(email, token);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        
        [HttpPost("banUser")]
        public async Task<ActionResult> BanUser(string id)
        {
            var result = await _userService.BanUserAsync(id);
            if (!result)
            {
                return NotFound();
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
                return NotFound();
            }

            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username and password are required.");
            }

            var result = await _userService.LoginAsync(userName, password);
            if (!result)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok("Login successful");

        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<ActionResult> Logout()
        {


            await _userService.LogoutAsync();
            return Ok("Logout successful.");
        }
        [HttpPost("forgetPassword")]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            var result = await _userService.ForgetPasswordAsync(email);
            if (!result)
            {
                return BadRequest("User was not found");
            }
            return Ok("Email was sent");
        }
        [HttpGet("ResetPassword")]
        public async Task<ActionResult<ResetPasswordForm>> ResetPasswordAbc([FromQuery]string email,[FromQuery] string token)
        {
            var restPasswordForm =await _userService.ResetPassswordAbc(email, token);
            if (restPasswordForm==null)
            {
                return BadRequest("UserName or Token is not valid");
            }

            return Ok(restPasswordForm);
        }
        [HttpPost("ResetPasswordImp")]
        public async Task<ActionResult> ResetPassword([FromBody]ResetPasswordForm resetPasswordForm)
        {
            var result = await _userService.ResetPasswordAsync(resetPasswordForm);
            if (!result)
            {
                return BadRequest("UserName or Token is not valid");
            }

            return Ok("Password reset successful");
        }
       
    }
}
