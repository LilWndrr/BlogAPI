using BlogAPI.DTOs;
using BlogAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Services.Interfaces;

public interface ICommentLikeService
{
    Task<IEnumerable<CommentLikeGetDto>?> GetCommentsLikesByCommentIdAsync(long commentId);
    Task<IEnumerable<CommentLikeGetDto>?> GetUserCommentLikesAsync(string id);
    Task<bool> PostCommentLikeAsync(CommentLikePostDto commentLike);
    Task<bool> DeleteCommentLikeAsync(long commentId,string userId);

}