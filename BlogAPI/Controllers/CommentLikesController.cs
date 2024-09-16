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

    // GET: api/CommentLikes/GetPostLikes
    [HttpGet("GetPostLikes/{commentId}")]
    public async Task<ActionResult<IEnumerable<CommentLikeGetDto>>> GetCommentLikesByCommentId(long commentId)
    {
        var result = await _commentLikeService.GetCommentsLikesByCommentIdAsync(commentId);
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    // GET: api/CommentLikes/GetUserLikes/{userId}
    [HttpGet("GetUserLikes/{userId}")]
    public async Task<ActionResult<IEnumerable<CommentLikeGetDto>>> GetUserCommentLikes(string userId)
    {
        var result = await _commentLikeService.GetUserCommentLikesAsync(userId);
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    // POST: api/CommentLikes
    [HttpPost]
    public async Task<ActionResult> PostCommentLike([FromBody] CommentLikePostDto commentLikePost)
    {
        commentLikePost.UserId =User.FindFirst("uid")?.Value;

        var result = await _commentLikeService.PostCommentLikeAsync(commentLikePost);
        if (!result.Success)
        {
            return Problem(result.ErrorMessage);
        }

        return Ok();
    }

    // DELETE: api/CommentLikes
    [HttpDelete]
    public async Task<IActionResult> DeleteCommentLike(long commentId)
    {
        var userId =User.FindFirst("uid")?.Value;

        var result = await _commentLikeService.DeleteCommentLikeAsync(commentId, userId);
        if (!result.Success)
        {
            return Problem(result.ErrorMessage);
        }

        return Ok("Comment like removed successfully.");
    }
}

}
