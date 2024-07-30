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
    public class CommentLikesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public CommentLikesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/CommentLikes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentLike>>> GetCommentLikes()
        {
          if (_context.CommentLikes == null)
          {
              return NotFound();
          }
            return await _context.CommentLikes.ToListAsync();
        }

        // GET: api/CommentLikes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentLike>> GetCommentLike(string id)
        {
          if (_context.CommentLikes == null)
          {
              return NotFound();
          }
            var commentLike = await _context.CommentLikes.FindAsync(id);

            if (commentLike == null)
            {
                return NotFound();
            }

            return commentLike;
        }

        // PUT: api/CommentLikes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommentLike(string id, CommentLike commentLike)
        {
            if (id != commentLike.UserID)
            {
                return BadRequest();
            }

            _context.Entry(commentLike).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentLikeExists(id))
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

        // POST: api/CommentLikes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CommentLike>> PostCommentLike(CommentLike commentLike)
        {
          if (_context.CommentLikes == null)
          {
              return Problem("Entity set 'ApplicationContext.CommentLikes'  is null.");
          }
            commentLike.UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _context.CommentLikes.Add(commentLike);
            var comment =_context.Comments.FirstOrDefault(c=>c.Id== commentLike.CommentId);
            comment.LikesCount++;
            _context.Comments.Update(comment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CommentLikeExists(commentLike.UserID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCommentLike", new { id = commentLike.UserID }, commentLike);
        }

        // DELETE: api/CommentLikes/5
        [HttpDelete]
        public async Task<IActionResult> DeleteCommentLike(long CommentId)
        {
            if (_context.CommentLikes == null)
            {
                return NotFound();
            }
            var UserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var commentLike = await _context.CommentLikes.Where(cl=>cl.UserID==UserID &&cl.CommentId==CommentId).FirstOrDefaultAsync();
            if (commentLike == null)
            {
                return NotFound();
            }

            _context.CommentLikes.Remove(commentLike);
            var comment = _context.Comments.FirstOrDefault(c => c.Id == commentLike.CommentId);
            comment.LikesCount--;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentLikeExists(string id)
        {
            return (_context.CommentLikes?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}
