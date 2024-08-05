using BlogAPI.DTOs;

namespace BlogAPI.Services.Interfaces;

public interface IPostLikeService
{
    Task<IEnumerable<PostLikeGetDto>?> GetPostLikesByPostIdAsync(long postId);
    Task<IEnumerable<PostLikeGetDto>?> GetUserPostLikesAsync(string id);
    Task<bool> CreatePostLikeAsync(PostLikeCreateDto postLikeCreateDto);
    Task<bool> DeletePostLikeAsync(long postId,string userId);
}