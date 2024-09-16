using BlogAPI.Model;

namespace BlogAPI.Repositories;

public interface ICommentRepository
{
    Task<Comment> GetCommentByIdAsync(long id);
    Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(long postId);
    Task<IEnumerable<Comment>> GetCommentsByParentIdAsync(long parentId);
    Task AddCommentAsync(Comment comment);
    Task UpdateCommentAsync(Comment comment);
    Task SaveChangesAsync();
    Task<Comment> GetParentCommentAsync(long? commentId);
    
}
