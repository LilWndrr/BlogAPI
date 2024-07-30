using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.Model;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagPostsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public TagPostsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/TagPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagPost>>> GetTagPosts()
        {
          if (_context.TagPosts == null)
          {
              return NotFound();
          }
            return await _context.TagPosts.ToListAsync();
        }

        // GET: api/TagPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TagPost>> GetTagPost(int id)
        {
          if (_context.TagPosts == null)
          {
              return NotFound();
          }
            var tagPost = await _context.TagPosts.FindAsync(id);

            if (tagPost == null)
            {
                return NotFound();
            }

            return tagPost;
        }

        // PUT: api/TagPosts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTagPost(int id, TagPost tagPost)
        {
            if (id != tagPost.TagId)
            {
                return BadRequest();
            }

            _context.Entry(tagPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagPostExists(id))
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

        // POST: api/TagPosts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TagPost>> PostTagPost(TagPost tagPost)
        {
          if (_context.TagPosts == null)
          {
              return Problem("Entity set 'ApplicationContext.TagPosts'  is null.");
          }
            _context.TagPosts.Add(tagPost);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TagPostExists(tagPost.TagId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTagPost", new { id = tagPost.TagId }, tagPost);
        }

        // DELETE: api/TagPosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTagPost(int id)
        {
            if (_context.TagPosts == null)
            {
                return NotFound();
            }
            var tagPost = await _context.TagPosts.FindAsync(id);
            if (tagPost == null)
            {
                return NotFound();
            }

            _context.TagPosts.Remove(tagPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TagPostExists(int id)
        {
            return (_context.TagPosts?.Any(e => e.TagId == id)).GetValueOrDefault();
        }
    }
}
