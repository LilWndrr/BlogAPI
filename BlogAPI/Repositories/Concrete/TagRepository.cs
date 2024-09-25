using BlogAPI.Data;
using BlogAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Repositories.Concrete;

public class TagRepository : ITagRepository
{
    private readonly ApplicationContext _context; 

    public TagRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        return await _context.Tags.FindAsync(name);


    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _context.Tags.ToListAsync();
    }

    public async Task AddTagAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTagAsync(Tag tag)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(string name)
    {
        var tag = await GetTagByNameAsync(name);
        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}
