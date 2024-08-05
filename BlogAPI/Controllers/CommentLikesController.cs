using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Services.Concrete;
using BlogAPI.Services.Interfaces;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentLikesController : ControllerBase
    {
       
        private readonly ICommentLikeService _commentLikeService;

        public CommentLikesController(ICommentLikeService commentLikeService)
        {
            
            _commentLikeService = commentLikeService;
        }

        // GET: api/CommentLikes
        [HttpGet("GetPostLikes")]
        public async Task<ActionResult<IEnumerable<CommentLikeGetDto>>> GetCommentLikesByPostId( long id)
        {
            var commentLikes = await _commentLikeService.GetCommentsLikesByCommentIdAsync(id);
            if (commentLikes == null)
            {
                return BadRequest();
            }

            return Ok(commentLikes);
        }

        // GET: api/CommentLikes/5
        [HttpGet("GetUserLikes")]
        public async Task<ActionResult<IEnumerable<CommentLikeGetDto>>> GetUserCommentLikes(string id)
        {
            var commentLike = await _commentLikeService.GetUserCommentLikesAsync(id);
            if (commentLike == null)
            {
                return BadRequest();
            }
            
            return Ok(commentLike);
        }

       

        // POST: api/CommentLikes
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostCommentLike([FromBody]CommentLikePostDto commentLikePost)
        {
          
            commentLikePost.UserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await _commentLikeService.PostCommentLikeAsync(commentLikePost);
            if (!result)
            {
                return Problem("Smth went wrong, try it later");
            }

            return Ok();
        }

        // DELETE: api/CommentLikes/5
        [HttpDelete]
        public async Task<IActionResult> DeleteCommentLike(long commentId)
        {
           
            
            var result = await _commentLikeService.DeleteCommentLikeAsync(commentId,User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (!result)
            {
                return Problem("Smth went wrong, try it later");
            }

            return Ok();
        }

       
    }
}
