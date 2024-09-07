using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Mappers;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class CommentService : ICommentService
{
    private readonly ApplicationContext _context;

    public CommentService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<CommentGetDto>> GetCommentAsync(long id)
    {
        if (_context.Comments == null)
        {
            return ServiceResult<CommentGetDto>.FailureResult("Comments context is not available.");
        }

        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return ServiceResult<CommentGetDto>.FailureResult("Comment not found.");
        }

        return ServiceResult<CommentGetDto>.SuccessResult(comment.ToDto());
    }

    public async Task<ServiceResult<CommentGetDto>> PutCommentAsync(long id, CommentCreateDto commentCreateDto)
    {
        if (_context.Comments == null)
        {
            return ServiceResult<CommentGetDto>.FailureResult("Comments context is not available.");
        }

        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return ServiceResult<CommentGetDto>.FailureResult("Comment not found.");
        }

        comment.Content = commentCreateDto.Content;
        comment.UpdateDateTime = DateTime.Now;

        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();

        return ServiceResult<CommentGetDto>.SuccessResult(comment.ToDto());
    }

    public async Task<ServiceResult<IEnumerable<CommentGetDto>>> GetCommentByPostIdAsync(long postId)
    {
        if (_context.Comments == null)
        {
            return ServiceResult<IEnumerable<CommentGetDto>>.FailureResult("Comments context is not available.");
        }

        var comments = await _context.Comments
            .Where(c => c.PostId == postId && c.CommentId == null)
            .ToListAsync();

        return ServiceResult<IEnumerable<CommentGetDto>>.SuccessResult(comments.Select(comment => comment.ToDto()));
    }

    public async Task<ServiceResult<IEnumerable<CommentGetDto>>> GetCommentByParentIdAsync(long parentId)
    {
        if (_context.Comments == null)
        {
            return ServiceResult<IEnumerable<CommentGetDto>>.FailureResult("Comments context is not available.");
        }

        var comments = await _context.Comments
            .Where(c => c.CommentId == parentId)
            .ToListAsync();

        return ServiceResult<IEnumerable<CommentGetDto>>.SuccessResult(comments.Select(comment => comment.ToDto()));
    }

    public async Task<ServiceResult<CommentGetDto>> PostCommentAsync(CommentCreateDto commentCreateDto)
    {
        if (_context.Comments == null)
        {
            return ServiceResult<CommentGetDto>.FailureResult("Comments context is not available.");
        }

        Comment comment = new Comment
        {
            Content = commentCreateDto.Content,
            CommentId = commentCreateDto.CommentId,
            UserID = commentCreateDto.UserId
        };

        if (commentCreateDto.CommentId != null)
        {
            var parentComment = await _context.Comments.FindAsync(commentCreateDto.CommentId);
            if (parentComment != null)
            {
                comment.PostId = parentComment.PostId;
                parentComment.CommentCount++;
                _context.Comments.Update(parentComment);
            }
        }
        else
        {
            comment.PostId = await _context.Posts
                .Where(p => p.Id == comment.PostId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();
        }

        var post = await _context.Posts.FindAsync(comment.PostId);
        if (post != null)
        {
            post.CommentCount++;
            _context.Posts.Update(post);
        }

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return ServiceResult<CommentGetDto>.SuccessResult(comment.ToDto());
    }

    public async Task<ServiceResult<bool>> DeleteCommentAsync(long id)
    {
        if (_context.Comments == null)
        {
            return ServiceResult<bool>.FailureResult("Comments context is not available.");
        }

        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return ServiceResult<bool>.FailureResult("Comment not found.");
        }

        comment.IsDeleted = true;
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }
}
