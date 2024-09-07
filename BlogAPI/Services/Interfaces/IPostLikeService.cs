using BlogAPI.DTOs;
using BlogAPI.HelperServices;

namespace BlogAPI.Services.Interfaces;

public interface IPostLikeService
{
    Task<ServiceResult<IEnumerable<PostLikeGetDto>>> GetPostLikesByPostIdAsync(long postId);
    Task<ServiceResult<IEnumerable<PostLikeGetDto>>> GetUserPostLikesAsync(string id);
    Task<ServiceResult<bool>> CreatePostLikeAsync(PostLikeCreateDto postLikeCreateDto);
    Task<ServiceResult<bool>> DeletePostLikeAsync(long postId,string userId);
}