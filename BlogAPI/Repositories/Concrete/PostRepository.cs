using BlogAPI.Data;
using BlogAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlogAPI.Repositories.Concrete;

public class PostRepository : IPostRepository
{
    private readonly ApplicationContext _context;
    private readonly IMemoryCache _memoryCache;

    public PostRepository(ApplicationContext context,IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

  

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _context.Posts.AsNoTracking()
           
            .Include(p => p.PostLikes).ThenInclude(pl => pl.User)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Include(p => p.Authors).ThenInclude(up => up.Author)
            .Include(p => p.TagPosts).ThenInclude(tp => tp.Tag)
            .ToListAsync();
    }

    public async Task<Post> GetPostByIdAsync(long? id)
    {
        
        string key = $"post-{id}";
        
        return await _memoryCache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return _context.Posts
                    .Include(p => p.PostLikes).ThenInclude(pl => pl.User)
                    .Include(p => p.Comments).ThenInclude(c => c.User)
                    .Include(p => p.Authors).ThenInclude(up => up.Author)
                    .Include(p => p.TagPosts).ThenInclude(tp => tp.Tag)
                    .FirstOrDefaultAsync(p => p.Id == id);;
            });
    }

    public async Task<IEnumerable<Post>> GetPostsByTagIdAsync(string tagId)
    {
        
        string key = $"post-{tagId}";
        
        return await _memoryCache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return _context.TagPosts.AsNoTracking()
                    .Where(tp => tp.TagId == tagId)
                    .Select(tp => tp.Post)
                    .Where(p => !p.isBanned)
                    .ToListAsync();;
            });
    }

    public async Task AddPostAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task<UserPost> GetUserPostAsync(long postId, string userId)
    {
       
        string key = $"post-{postId},{userId}";
        
        return await _memoryCache.GetOrCreateAsync(key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return _context.UserPosts.FirstOrDefaultAsync(up => up.PostId == postId && up.AuthorId == userId);
            });
    }

    public async Task AddUserPostAsync(UserPost userPost)
    {
        _context.UserPosts.Add(userPost);
        await _context.SaveChangesAsync();
    }

    public async Task<Tag> GetTagPostAsync(string tagId)
    {
        return await _context.Tags.FindAsync(tagId);
    }

    public async Task AddTagPostAsync(TagPost tagPost)
    {
        _context.TagPosts.Add(tagPost);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
