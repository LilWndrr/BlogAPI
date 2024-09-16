using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Mappers;
using BlogAPI.Model;
using BlogAPI.Repositories;
using BlogAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;

    public CommentService(ICommentRepository commentRepository,IPostRepository postRepository)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
    }

    public async Task<ServiceResult<CommentGetDto>> GetCommentAsync(long id)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return ServiceResult<CommentGetDto>.FailureResult("Comment not found.");
        }

        return ServiceResult<CommentGetDto>.SuccessResult(comment.ToDto());
    }

    public async Task<ServiceResult<CommentGetDto>> PutCommentAsync(long id, CommentCreateDto commentCreateDto)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return ServiceResult<CommentGetDto>.FailureResult("Comment not found.");
        }

        comment.Content = commentCreateDto.Content;
        comment.UpdateDateTime = DateTime.Now;

        await _commentRepository.UpdateCommentAsync(comment);
        return ServiceResult<CommentGetDto>.SuccessResult(comment.ToDto());
    }

    public async Task<ServiceResult<IEnumerable<CommentGetDto>>> GetCommentByPostIdAsync(long postId)
    {
        var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);
        var commentDtos = comments.Select(comment => comment.ToDto());
        return ServiceResult<IEnumerable<CommentGetDto>>.SuccessResult(commentDtos);
    }

    public async Task<ServiceResult<IEnumerable<CommentGetDto>>> GetCommentByParentIdAsync(long parentId)
    {
        var comments = await _commentRepository.GetCommentsByParentIdAsync(parentId);
        var commentDtos = comments.Select(comment => comment.ToDto());
        return ServiceResult<IEnumerable<CommentGetDto>>.SuccessResult(commentDtos);
    }

    public async Task<ServiceResult<CommentGetDto>> PostCommentAsync(CommentCreateDto commentCreateDto)
    {
        var comment = new Comment 
        {
            Content = commentCreateDto.Content,
            CommentId = commentCreateDto.CommentId,
            UserID = commentCreateDto.UserId,
            CreatedDateTime = DateTime.Now
        };

        var parentComment = await _commentRepository.GetParentCommentAsync(commentCreateDto.CommentId);
        if (parentComment != null)
        {
            comment.PostId = parentComment.PostId;
            parentComment.CommentCount++;
            await _commentRepository.UpdateCommentAsync(parentComment);
        }
        else
        {
            var post = await _postRepository.GetPostByIdAsync(comment.PostId);
            if (post != null)
            {
                post.CommentCount++;
                await _postRepository.UpdatePostAsync(post);
            }
        }

        await _commentRepository.AddCommentAsync(comment);
        return ServiceResult<CommentGetDto>.SuccessResult(comment.ToDto());
    }

    public async Task<ServiceResult<bool>> DeleteCommentAsync(long id)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return ServiceResult<bool>.FailureResult("Comment not found.");
        }

        comment.IsDeleted = true;
        var post = await _postRepository.GetPostByIdAsync(comment.PostId);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        post.CommentCount--;
        await _postRepository.UpdatePostAsync(post);
        await _commentRepository.UpdateCommentAsync(comment);
        
        return ServiceResult<bool>.SuccessResult(true);
    }
}
