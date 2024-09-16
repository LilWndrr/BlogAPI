using BlogAPI.Model;

namespace BlogAPI.Repositories;

public interface ICommentLikeRepository
{
    Task<IEnumerable<CommentLike>> GetCommentLikesByCommentIdAsync(long commentId);
    Task<IEnumerable<CommentLike>> GetCommentLikesByUserIdAsync(string userId);
    Task<CommentLike> FindCommentLikeAsync(long commentId, string userId);
    
    Task AddCommentLikeAsync(CommentLike commentLike);
    Task RemoveCommentLikeAsync(CommentLike commentLike);
    
    Task SaveChangesAsync();
}
