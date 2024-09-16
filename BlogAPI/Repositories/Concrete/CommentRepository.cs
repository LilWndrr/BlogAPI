using BlogAPI.Data;
using BlogAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlogAPI.Repositories.Concrete;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationContext _context;
    private readonly IMemoryCache _memoryCache;

    public CommentRepository(ApplicationContext context,IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    public async Task<Comment> GetCommentByIdAsync(long id)
    {

        string key = $"comment-{id}";
        
        return await _memoryCache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return  _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            });
    }

    public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(long postId)
    {
        
        string key = $"comment-{postId}";
        
        return await _memoryCache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return  _context.Comments.AsNoTracking()
                    .Where(c => c.PostId == postId && c.CommentId == null)
                    .OrderByDescending(c => c.CreatedDateTime)
                    .ToListAsync();
            });
    }

    public async Task<IEnumerable<Comment>> GetCommentsByParentIdAsync(long parentId)
    {
        
        string key = $"comment-{parentId}";
        
        return await _memoryCache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return  _context.Comments.AsNoTracking()
                    .Where(c => c.CommentId == parentId)
                    .OrderByDescending(c => c.CreatedDateTime)
                    .ToListAsync();
            });
    }

    public async Task AddCommentAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Comment> GetParentCommentAsync(long? commentId)
    {
        if (commentId == null) return null;
        return await _context.Comments.FindAsync(commentId);
    }

 
}
