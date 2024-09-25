using BlogAPI.Model;

namespace BlogAPI.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetTagByNameAsync(string name);
    Task<List<Tag>> GetAllTagsAsync();
    Task AddTagAsync(Tag tag);
    Task UpdateTagAsync(Tag tag);
    Task DeleteTagAsync(string name);
}
