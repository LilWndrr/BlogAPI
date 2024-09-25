using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.Model;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public TagsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
          if (_context.Tags == null)
          {
              return NotFound();
          }
          return await _context.Tags.ToListAsync();
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
          if (_context.Tags == null)
          {
              return NotFound();
          }
          var tag = await _context.Tags.FindAsync(id);

          if (tag == null)
          {
              return NotFound();
          }

          return tag;
        }

        

        // POST: api/Tags
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tag>> PostTag(Tag tag)
        {
          if (_context.Tags == null)
          {
              return Problem("Entity set 'ApplicationContext.Tags'  is null.");
          }
          _context.Tags.Add(tag);
          await _context.SaveChangesAsync();

          return CreatedAtAction("GetTag", new { id = tag.Name }, tag);
        }

      

        
    }
}
