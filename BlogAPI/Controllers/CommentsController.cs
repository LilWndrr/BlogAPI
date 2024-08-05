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
        public async Task<ActionResult<CommentGetDto>?> GetComment(long id)
        {
            var comment = await _commentService.GetCommentAsync(id);

            if (comment != null) return Ok(comment);
            return null;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<CommentGetDto>> PutComment(long id,[FromBody] CommentCreateDto commentCreate)
        {
            var comment = await _commentService.PutCommentAsync(id, commentCreate);
            return Ok(comment);
        }

        [HttpGet("GetByPostID")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentByPostId(long id)
        {
            var comments = await _commentService.GetCommentByPostIdAsync(id);

            return Ok(comments);
        }
        [HttpGet("GetByParentID/{id}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentByParentId(long id)
        {
            var comments = await _commentService.GetCommentByParentIdAsync(id);

            return Ok(comments);
        }

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CommentGetDto>> PostComment(CommentCreateDto commentCreate)
        {
         
            
            commentCreate.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var comment = await _commentService.PostCommentAsync(commentCreate);
            return Ok(comment);

        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(long id)
        {
           
            //Necessary for complete deleting from database
            //await RemoveAllSubCommentsAsync(comment.Id); 
            var result =await _commentService.DeleteCommentAsync(id);
            if (result)
            {
                return Ok("Deleting is completed");
            }

            return BadRequest("Smth went wrong");
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
