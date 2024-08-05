using BlogAPI.DTOs;
using BlogAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Services.Interfaces;

public interface ICommentService
{
    Task<CommentGetDto?> GetCommentAsync(long id);
    Task<CommentGetDto?>? PutCommentAsync(long id, CommentCreateDto commentCreateDto);
    Task<IEnumerable<CommentGetDto>?> GetCommentByPostIdAsync(long postId);
    Task<IEnumerable<CommentGetDto>?> GetCommentByParentIdAsync(long parentId);
    Task<CommentGetDto?> PostCommentAsync(CommentCreateDto commentCreateDto);
    Task<bool> DeleteCommentAsync(long id);

}