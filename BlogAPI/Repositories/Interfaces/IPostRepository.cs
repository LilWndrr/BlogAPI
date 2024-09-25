using BlogAPI.Model;

namespace BlogAPI.Repositories;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetAllPostsAsync();
    Task<Post> GetPostByIdAsync(long? id);
    Task<IEnumerable<Post>> GetPostsByTagIdAsync(string tagId);
    Task AddPostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task<UserPost> GetUserPostAsync(long postId, string userId);
    Task AddUserPostAsync(UserPost userPost);
    Task<Tag> GetTagPostAsync(string tagId);
    Task AddTagPostAsync(TagPost tagPost);
    Task SaveChangesAsync();
}
