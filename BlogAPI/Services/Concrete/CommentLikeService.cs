using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class CommentLikeService:ICommentLikeService

{
    private readonly ApplicationContext _context;

    public CommentLikeService(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<CommentLikeGetDto>?> GetCommentsLikesByCommentIdAsync(long commentId)
    {
        if (_context.CommentLikes != null)
        {
            var commentLikes = await _context.CommentLikes.Where(cl => cl.CommentId == commentId).ToListAsync();
            if (commentLikes !=null)
            {
                return commentLikes.Select(commentLike=> new CommentLikeGetDto
                {
                    UserId = commentLike.UserID,
                    DateCreated = commentLike.DateCreated
                });
            }
        }

        return null;
    }

    public async Task<IEnumerable<CommentLikeGetDto>?> GetUserCommentLikesAsync(string id)
    {
        if (_context.CommentLikes != null)
        {
            var commentLikes = await _context.CommentLikes.Where(cl => cl.UserID == id).ToListAsync();
            if (commentLikes != null)
            {
                return commentLikes.Select(commentLike => new CommentLikeGetDto
                {
                    CommentId = commentLike.CommentId,
                    DateCreated = commentLike.DateCreated
                });
            }
        }

        return null;
    }

    public async Task<bool> PostCommentLikeAsync(CommentLikePostDto commentLikeDto)
    {
        if (_context.CommentLikes != null)
        {
            CommentLike commentLike = new CommentLike
            {
                UserID = commentLikeDto.UserId,
                CommentId = commentLikeDto.CommentId,
                DateCreated = DateTime.Now
            };
            _context.CommentLikes.Add(commentLike);
            if (_context.Comments != null)
            {
                var comment = _context.Comments.FirstOrDefault(c => c.Id == commentLike.CommentId);
                if (comment != null)
                {
                    comment.LikesCount++;
                    _context.Comments.Update(comment);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteCommentLikeAsync(long commentId, string userId)
    {
        if (_context.CommentLikes != null)
        {
            var commentLike = await _context.CommentLikes.FindAsync(commentId, userId);
            if (commentLike != null)
            {
                if (_context.Comments != null)
                {
                    var comment = _context.Comments.FirstOrDefault(c => c.Id == commentLike.CommentId);
                    if (comment != null)
                    {
                        comment.LikesCount--;
                        _context.Comments.Update(comment);
                    }
                }

                _context.CommentLikes.Remove(commentLike);
                await _context.SaveChangesAsync();
            }
        }
        return false;
    }
}