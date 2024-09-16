using BlogAPI.Model;

namespace BlogAPI.Repositories;

public interface IPostLikeRepository
{
    Task<IEnumerable<PostLike>> GetPostLikesByPostIdAsync(long postId);
    Task<IEnumerable<PostLike>> GetUserPostLikesAsync(string userId);
    Task AddPostLikeAsync(PostLike postLike);
    Task<PostLike> FindPostLikeAsync(long postId, string userId);
    Task RemovePostLikeAsync(PostLike postLike);
   
    Task SaveChangesAsync();
}
