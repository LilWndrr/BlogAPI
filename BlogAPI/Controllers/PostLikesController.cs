using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.Model;
using System.Security.Claims;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostLikesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PostLikesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/PostLikes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostLike>>> GetPostLikes()
        {
          if (_context.PostLikes == null)
          {
              return NotFound();
          }
            return await _context.PostLikes.ToListAsync();
        }

        // GET: api/PostLikes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostLike>> GetPostLike(string id)
        {
          if (_context.PostLikes == null)
          {
              return NotFound();
          }
            var postLike = await _context.PostLikes.FindAsync(id);

            if (postLike == null)
            {
                return NotFound();
            }

            return postLike;
        }

        // PUT: api/PostLikes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostLike(string id, PostLike postLike)
        {
            if (id != postLike.UserID)
            {
                return BadRequest();
            }

            _context.Entry(postLike).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostLikeExists(id))
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

        // POST: api/PostLikes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostLike>> PostPostLike(PostLike postLike)
        {
          if (_context.PostLikes == null)
          {
              return Problem("Entity set 'ApplicationContext.PostLikes'  is null.");
          }
            postLike.UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _context.PostLikes.Add(postLike);
            var post=_context.Posts.FirstOrDefault(p=>p.Id==postLike.PostId);
            post.LikeCount++;
            _context.Posts.Update(post);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PostLikeExists(postLike.UserID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPostLike", new { id = postLike.UserID }, postLike);
        }

        // DELETE: api/PostLikes/5
        [HttpDelete]
        public async Task<IActionResult> DeletePostLike(long postID)
        {
            if (_context.PostLikes == null)
            {
                return NotFound();
            }
            var userId= User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var postLike = await _context.PostLikes.Where(pl=>pl.UserID==userId&&pl.PostId==postID).FirstOrDefaultAsync();
            if (postLike == null)
            {
                return NotFound();
            }
            var post = _context.Posts.FirstOrDefault(p => p.Id == postLike.PostId);
            post.LikeCount--;
            _context.Posts.Update(post);
            _context.PostLikes.Remove(postLike);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostLikeExists(string id)
        {
            return (_context.PostLikes?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}
