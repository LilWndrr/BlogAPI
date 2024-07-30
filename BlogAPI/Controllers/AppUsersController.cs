using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.Model;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.Metrics;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUsersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AppUsersController(ApplicationContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/AppUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }
        

        // GET: api/AppUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetAppUser(string id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var appUser = await _context.Users.FindAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            return appUser;
        }

        // PUT: api/AppUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppUser(string id, AppUser appUser, string? CurrentPassword)
        {
            if (id != appUser.Id)
            {
                return BadRequest();
            }
            AppUser appUser1= _userManager.FindByIdAsync(id).Result;

           
            appUser1.BirthDate = appUser.BirthDate;
            appUser1.Email = appUser.Email;
            appUser1.FamilyName = appUser.FamilyName;
            appUser1.Gender = appUser.Gender;
            appUser1.MiddleName = appUser.MiddleName;
            appUser1.FamilyName = appUser.FamilyName;
            appUser1.Status = appUser.Status;
            appUser1.Password = appUser.Password;
            await _userManager.UpdateAsync(appUser1);
            if (CurrentPassword != null)
            {
                _userManager.ChangePasswordAsync(appUser1, CurrentPassword, appUser.Password).Wait();
            }
            return NoContent();
        }

        // POST: api/AppUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public  ActionResult<AppUser> PostAppUser(AppUser appUser)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'ApplicationContext.Users'  is null.");
          }

            var result= _userManager.CreateAsync(appUser, appUser.Password).Result;
            if (result.Succeeded)
            {
                return Ok("Posting saccessful");
            }
            
           
            

            return CreatedAtAction("GetAppUser", new { id = appUser.Id }, appUser);
        }

        // DELETE: api/AppUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppUser(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var appUser = await _context.Users.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            _context.Users.Remove(appUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username and password are required.");
            }

            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(appUser,password,false,false);

            if (signInResult.Succeeded)
            {
                
                return Ok();
            }

            return Unauthorized("Invalid username or password.");
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<ActionResult> Logout()
        {


            await _signInManager.SignOutAsync();
            return Ok("Logout successful.");
        }

        private bool AppUserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
