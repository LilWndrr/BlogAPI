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

        // GET: api/PostLikes/getByPost/{id}
        [HttpGet("getByPost/{id}")]
        public async Task<ActionResult<IEnumerable<PostLikeGetDto>>> GetPostLikesByPostId(long id)
        {
            var result = await _postLikeService.GetPostLikesByPostIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // GET: api/PostLikes/getByUser/{id}
        [HttpGet("getByUser/{id}")]
        public async Task<ActionResult<IEnumerable<PostLikeGetDto>>> GetUserPostLikes(string id)
        {
            var result = await _postLikeService.GetUserPostLikesAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // POST: api/PostLikes
        [HttpPost]
        public async Task<ActionResult> PostPostLike([FromBody] PostLikeCreateDto postLikeCreateDto)
        {
            postLikeCreateDto.UserId =User.FindFirst("uid")?.Value;

            var result = await _postLikeService.CreatePostLikeAsync(postLikeCreateDto);
            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok();
        }

        // DELETE: api/PostLikes/{postId}
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePostLike(long postId)
        {
            var userId =User.FindFirst("uid")?.Value;

            var result = await _postLikeService.DeletePostLikeAsync(postId, userId);
            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok("Post like removed successfully.");
        }
    }

}
