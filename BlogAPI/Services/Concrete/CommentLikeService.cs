using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Mappers;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class CommentLikeService : ICommentLikeService
{
    private readonly ApplicationContext _context;

    public CommentLikeService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<IEnumerable<CommentLikeGetDto>>> GetCommentsLikesByCommentIdAsync(long commentId)
    {
        if (_context.CommentLikes == null)
        {
            return ServiceResult<IEnumerable<CommentLikeGetDto>>.FailureResult("CommentLikes context is not available.");
        }

        var commentLikes = await _context.CommentLikes
            .Where(cl => cl.CommentId == commentId)
            .ToListAsync();

        return commentLikes.Any()
            ? ServiceResult<IEnumerable<CommentLikeGetDto>>.SuccessResult(commentLikes.Select(cl => cl.ToDto()))
            : ServiceResult<IEnumerable<CommentLikeGetDto>>.FailureResult("No likes found for the given comment.");
    }

    public async Task<ServiceResult<IEnumerable<CommentLikeGetDto>>> GetUserCommentLikesAsync(string userId)
    {
        if (_context.CommentLikes == null)
        {
            return ServiceResult<IEnumerable<CommentLikeGetDto>>.FailureResult("CommentLikes context is not available.");
        }

        var commentLikes = await _context.CommentLikes
            .Where(cl => cl.UserID == userId)
            .ToListAsync();

        return commentLikes.Any()
            ? ServiceResult<IEnumerable<CommentLikeGetDto>>.SuccessResult(commentLikes.Select(cl => cl.ToDto()))
            : ServiceResult<IEnumerable<CommentLikeGetDto>>.FailureResult("No likes found for the given user.");
    }

    public async Task<ServiceResult<bool>> PostCommentLikeAsync(CommentLikePostDto commentLikeDto)
    {
        if (_context.CommentLikes == null)
        {
            return ServiceResult<bool>.FailureResult("CommentLikes context is not available.");
        }

        CommentLike commentLike = commentLikeDto.ToEntity();
        _context.CommentLikes.Add(commentLike);

        var comment = await _context.Comments.FindAsync(commentLike.CommentId);
        if (comment != null)
        {
            comment.LikesCount++;
            _context.Comments.Update(comment);
        }

        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> DeleteCommentLikeAsync(long commentId, string userId)
    {
        if (_context.CommentLikes == null)
        {
            return ServiceResult<bool>.FailureResult("CommentLikes context is not available.");
        }

        var commentLike = await _context.CommentLikes.FindAsync(commentId, userId);
        if (commentLike == null)
        {
            return ServiceResult<bool>.FailureResult("Comment like not found.");
        }

        var comment = await _context.Comments.FindAsync(commentLike.CommentId);
        if (comment != null)
        {
            comment.LikesCount--;
            _context.Comments.Update(comment);
        }

        _context.CommentLikes.Remove(commentLike);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }
}
