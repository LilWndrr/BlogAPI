using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Data;
using BlogAPI.Model;
using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Services.Concrete;
using BlogAPI.Services.Interfaces;

namespace BlogAPI.Controllers
{
   [Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    // GET: api/Comments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentGetDto>> GetComment(long id)
    {
        var result = await _commentService.GetCommentAsync(id);
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Data);
    }

    // PUT: api/Comments/5
    [HttpPut("{id}")]
    public async Task<ActionResult<CommentGetDto>> PutComment(long id, [FromBody] CommentCreateDto commentCreate)
    {
        var result = await _commentService.PutCommentAsync(id, commentCreate);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Data);
    }

    [HttpGet("GetByPostId/{postId}")]
    public async Task<ActionResult<IEnumerable<CommentGetDto>>> GetCommentByPostId(long postId)
    {
        var result = await _commentService.GetCommentByPostIdAsync(postId);
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Data);
    }

    [HttpGet("GetByParentId/{parentId}")]
    public async Task<ActionResult<IEnumerable<CommentGetDto>>> GetCommentByParentId(long parentId)
    {
        var result = await _commentService.GetCommentByParentIdAsync(parentId);
        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Data);
    }

    // POST: api/Comments
    [HttpPost]
    public async Task<ActionResult<CommentGetDto>> PostComment([FromBody] CommentCreateDto commentCreate)
    {
        commentCreate.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _commentService.PostCommentAsync(commentCreate);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Data);
    }

    // DELETE: api/Comments/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(long id)
    {
        var result = await _commentService.DeleteCommentAsync(id);
        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok("Comment deleted successfully.");
    }



        //Necessary for cascade deleting all subcomments of comment
    /*    private async Task RemoveAllSubCommentsAsync( long CommentId)
        {
            
            var subcomments = await _context.Comments
                .Where(c => c.CommentId == CommentId)
                .ToListAsync();


            foreach (var subcomment in subcomments)
            {
                await RemoveAllSubCommentsAsync( subcomment.Id);
                _context.Comments.Remove(subcomment);
            }

            await _context.SaveChangesAsync();


        }*/
        
    }
}
