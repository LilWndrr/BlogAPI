using BlogAPI.Data;
using BlogAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Repositories.Concrete;

public class CommentLikeRepository : ICommentLikeRepository
{
    private readonly ApplicationContext _context;

    public CommentLikeRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CommentLike>> GetCommentLikesByCommentIdAsync(long commentId)
    {
        return await _context.CommentLikes.AsNoTracking()
            .Where(cl => cl.CommentId == commentId)
            .OrderByDescending(cl => cl.DateCreated)
            .ToListAsync();
    }

    public async Task<IEnumerable<CommentLike>> GetCommentLikesByUserIdAsync(string userId)
    {
        return await _context.CommentLikes.AsNoTracking()
            .Where(cl => cl.UserID == userId)
            .ToListAsync();
    }

    public async Task<CommentLike> FindCommentLikeAsync(long commentId, string userId)
    {
        return await _context.CommentLikes.FindAsync(commentId, userId);
    }

  

    public async Task AddCommentLikeAsync(CommentLike commentLike)
    {
        _context.CommentLikes.Add(commentLike);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveCommentLikeAsync(CommentLike commentLike)
    {
        _context.CommentLikes.Remove(commentLike);
        await _context.SaveChangesAsync();
    }

 

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
