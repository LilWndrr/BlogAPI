using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Services.Interfaces;

public interface ICommentLikeService
{
    Task<ServiceResult<IEnumerable<CommentLikeGetDto>>> GetCommentsLikesByCommentIdAsync(long commentId);
    Task<ServiceResult<IEnumerable<CommentLikeGetDto>>> GetUserCommentLikesAsync(string id);
    Task<ServiceResult<bool>> PostCommentLikeAsync(CommentLikePostDto commentLike);
    Task<ServiceResult<bool>> DeleteCommentLikeAsync(long commentId,string userId);

}