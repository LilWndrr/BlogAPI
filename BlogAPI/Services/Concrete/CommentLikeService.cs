using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Mappers;
using BlogAPI.Model;
using BlogAPI.Repositories;
using BlogAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class CommentLikeService : ICommentLikeService
{
    private readonly ICommentLikeRepository _commentLikeRepository;
    private readonly ICommentRepository _commentRepository;
    

    public CommentLikeService(ICommentLikeRepository commentLikeRepository,ICommentRepository commentRepository)
    {
        _commentLikeRepository = commentLikeRepository;
        _commentRepository = commentRepository;
    }

    public async Task<ServiceResult<IEnumerable<CommentLikeGetDto>>> GetCommentsLikesByCommentIdAsync(long commentId)
    {
        var commentLikes = await _commentLikeRepository.GetCommentLikesByCommentIdAsync(commentId);

        return commentLikes.Any()
            ? ServiceResult<IEnumerable<CommentLikeGetDto>>.SuccessResult(commentLikes.Select(cl => cl.ToDto()))
            : ServiceResult<IEnumerable<CommentLikeGetDto>>.FailureResult("No likes found for the given comment.");
    }

    public async Task<ServiceResult<IEnumerable<CommentLikeGetDto>>> GetUserCommentLikesAsync(string userId)
    {
        var commentLikes = await _commentLikeRepository.GetCommentLikesByUserIdAsync(userId);

        return commentLikes.Any()
            ? ServiceResult<IEnumerable<CommentLikeGetDto>>.SuccessResult(commentLikes.Select(cl => cl.ToDto()))
            : ServiceResult<IEnumerable<CommentLikeGetDto>>.FailureResult("No likes found for the given user.");
    }

    public async Task<ServiceResult<bool>> PostCommentLikeAsync(CommentLikePostDto commentLikeDto)
    {
        var commentLike = commentLikeDto.ToEntity();

        await _commentLikeRepository.AddCommentLikeAsync(commentLike);

        var comment = await _commentRepository.GetCommentByIdAsync(commentLike.CommentId);
        if (comment != null)
        {
            comment.LikesCount++;
            await _commentRepository.UpdateCommentAsync(comment);
        }

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> DeleteCommentLikeAsync(long commentId, string userId)
    {
        var commentLike = await _commentLikeRepository.FindCommentLikeAsync(commentId, userId);
        if (commentLike == null)
        {
            return ServiceResult<bool>.FailureResult("Comment like not found.");
        }

        var comment = await _commentRepository.GetCommentByIdAsync(commentId);
        if (comment != null)
        {
            comment.LikesCount--;
            await _commentRepository.UpdateCommentAsync(comment);
        }

        await _commentLikeRepository.RemoveCommentLikeAsync(commentLike);
        return ServiceResult<bool>.SuccessResult(true);
    }
}
