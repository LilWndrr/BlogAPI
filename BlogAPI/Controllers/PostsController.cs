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
using System.Security.Claims;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PostsController(ApplicationContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
          if (_context.Posts == null)
          {
              return NotFound();
          }
          

           
            return await _context.Posts.Include(p=>p.Comments).ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(long id)
        {
          if (_context.Posts == null)
          {
              return NotFound();
          }


            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpGet("getByTag")]
        public async Task<ActionResult<List<Post>>> GetPostByTag(long id)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }


            var posts = await _context.TagPosts
            .Where(tp => tp.TagId == id)
            .Select(tp => tp.Post)  // This projects to the Post entity
            
            .ToListAsync();
            if (posts == null)
            {
                return NotFound();
            }

            return posts;
        }
        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(long id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
          if (_context.Posts == null)
          {
              return Problem("Entity set 'ApplicationContext.Posts'  is null.");
          }
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            if (post.CoAuhtorsIds != null)
            {
                foreach (var authorID in post.CoAuhtorsIds)
                {
                    AppUser author = await _userManager.FindByIdAsync(authorID);
                    if (author == null)
                    {
                        return Problem("You can not choose author that is not registered");
                    }
                    UserPost userPost = new UserPost();
                    userPost.AuthorId = authorID;
                    userPost.PostId = post.Id;
                    _context.UserPosts.Add(userPost);
                }
            }
            if (post.TagIds!=null)
            {
                foreach (var tagId in post.TagIds)
                {
                    Tag tag =  _context.Tags.FirstOrDefault(t=> t.Id==tagId);
                    if (tag == null)
                    {
                        return BadRequest();
                    }
                    TagPost tagPost = new TagPost();
                    tagPost.TagId = tagId;
                    tagPost.PostId = post.Id;
                    _context.TagPosts.Add(tagPost);

                }
            }

            UserPost userPost1 = new UserPost();
            userPost1.AuthorId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            userPost1.PostId = post.Id;
            _context.UserPosts.Add(userPost1);
            
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(long id)
        {
            return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
