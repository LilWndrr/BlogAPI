using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Services.Interfaces;

public interface ICommentService
{
    Task<ServiceResult<CommentGetDto>> GetCommentAsync(long id);
    Task<ServiceResult<CommentGetDto>> PutCommentAsync(long id, CommentCreateDto commentCreateDto);
    Task<ServiceResult<IEnumerable<CommentGetDto>>> GetCommentByPostIdAsync(long postId);
    Task<ServiceResult<IEnumerable<CommentGetDto>>> GetCommentByParentIdAsync(long parentId);
    Task<ServiceResult<CommentGetDto>> PostCommentAsync(CommentCreateDto commentCreateDto);
    Task<ServiceResult<bool>> DeleteCommentAsync(long id);

}