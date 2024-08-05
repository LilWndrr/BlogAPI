using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Services.Concrete;
using BlogAPI.Services.Interfaces;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        
      
        private readonly IPostService _postService;
        public PostsController( IPostService postService)
        {
            
          
            _postService = postService;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostGetDTO>>> GetPosts()
        {

            var posts  =  await _postService.GetAllPostsAsync();
            if (posts==null)
            {
                return NotFound();
            }
           
            return Ok(posts);
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostGetDTO>> GetPost(long id)
        {
         


            var post = await _postService.GetPostByIdAsync(id);
            if (post==null)
            {
                return NotFound();
            }
           

            return Ok(post);
        }

        [HttpGet("getPostByTag")]
        public async Task<ActionResult<IEnumerable<PostGetDTO>>> GetPostByTag(int id)
        {



            var posts = await _postService.GetPostsByTagIdAsync(id); // This projects to the Post entity
            
            
            if (posts == null)
            {
                return NotFound();
            }

            return Ok(posts);
        }
        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public  IActionResult PutPost(long id,[FromBody] PostCreateDTO post)
        {
            var result =  _postService.UpdatePostAsync(id, post).Result;
            if (!result )
            {
                return Problem("Smth went wrong");
            }

            return NoContent();
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostGetDTO>> PostPost([FromBody] PostCreateDTO postCreateDto)
        {
           
            postCreateDto.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var post = await _postService.CreatePostAsync(postCreateDto);
            if (post  == null)
            {
                return BadRequest("Error creating post.");
            }
            return  CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
          

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await _postService.DeletePostAsync(id, userId);
           
            if (!result)
            {
                return BadRequest();
            }
          

            return NoContent();
        }

        
    }
}
