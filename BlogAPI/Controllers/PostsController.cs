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

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostGetDTO>>> GetPosts()
        {
            var result = await _postService.GetAllPostsAsync();
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // GET: api/Posts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PostGetDTO>> GetPost(long id)
        {
            var result = await _postService.GetPostByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // GET: api/Posts/getPostByTag
        [HttpGet("getPostByTag")]
        public async Task<ActionResult<IEnumerable<PostGetDTO>>> GetPostByTag(int id)
        {
            var result = await _postService.GetPostsByTagIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // PUT: api/Posts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(long id, [FromBody] PostCreateDTO postCreateDto)
        {
            var result = await _postService.UpdatePostAsync(id, postCreateDto);
            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return NoContent();
        }

        // POST: api/Posts
        [HttpPost]
        public async Task<ActionResult<PostGetDTO>> PostPost([FromBody] PostCreateDTO postCreateDto)
        {
            postCreateDto.UserId =User.FindFirst("uid")?.Value;

            var result = await _postService.CreatePostAsync(postCreateDto);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetPost), new { id = result.Data.Id }, result.Data);
        }

        // DELETE: api/Posts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            var userId =User.FindFirst("uid")?.Value;

            var result = await _postService.DeletePostAsync(id, userId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }
    }
}

