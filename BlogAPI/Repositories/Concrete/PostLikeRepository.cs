using BlogAPI.Data;
using BlogAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlogAPI.Repositories.Concrete;

public class PostLikeRepository : IPostLikeRepository
{
    private readonly ApplicationContext _context;
    private readonly IMemoryCache _memoryCache;

    public PostLikeRepository(ApplicationContext context,IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    public async Task<IEnumerable<PostLike>> GetPostLikesByPostIdAsync(long postId)
    {
       
        string key = $"postLike-{postId}";
        
        return await _memoryCache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return _context.PostLikes.Where(pl => pl.PostId == postId).ToListAsync();
            });
    }

    public async Task<IEnumerable<PostLike>> GetUserPostLikesAsync(string userId)
    {
       
        string key = $"postLike-{userId}";
        
        return await _memoryCache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return _context.PostLikes.Where(pl => pl.UserId == userId).ToListAsync();
            });
    }

    public async Task AddPostLikeAsync(PostLike postLike)
    {
        _context.PostLikes.Add(postLike);
        await _context.SaveChangesAsync();
    }

    public async Task<PostLike> FindPostLikeAsync(long postId, string userId)
    {
        return await _context.PostLikes.FindAsync(postId, userId);
    }

    public async Task RemovePostLikeAsync(PostLike postLike)
    {
        _context.PostLikes.Remove(postLike);
        await _context.SaveChangesAsync();
    }

   

   

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
