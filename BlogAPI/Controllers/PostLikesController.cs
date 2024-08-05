using Microsoft.AspNetCore.Mvc;
using BlogAPI.Model;
using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Services.Concrete;
using BlogAPI.Services.Interfaces;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostLikesController : ControllerBase
    {
       
        private readonly IPostLikeService _postLikeService;


        public PostLikesController(IPostLikeService postLikeService)
        {
            
            _postLikeService = postLikeService;
        }

      
        // GET: api/PostLikes/5
        [HttpGet("getByPost")]
        public async Task<ActionResult<IEnumerable<PostLike>>> GetPostLikeByPostId(long id)
        {
            var postLikes = await _postLikeService.GetPostLikesByPostIdAsync(id);
            if (postLikes == null)
            {
                return BadRequest();
            }

            return Ok(postLikes);
        }
        [HttpGet("getByUser")]
        public async Task<ActionResult<IEnumerable<PostLike>>> GetPostLikeByUserId(string id)
        {
            var postLikes = await _postLikeService.GetUserPostLikesAsync(id);
            if (postLikes == null)
            {
                return BadRequest();
            }

            return Ok(postLikes);
        }

       

        // POST: api/PostLikes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostPostLike([FromBody] PostLikeCreateDto postLikeCreateDto)
        {
          
            postLikeCreateDto.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await _postLikeService.CreatePostLikeAsync(postLikeCreateDto);
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }

        // DELETE: api/PostLikes/5
        [HttpDelete]
        public async Task<IActionResult> DeletePostLike(long postId)
        {
            
            var userId= User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await _postLikeService.DeletePostLikeAsync(postId, userId);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

      
    }
}
