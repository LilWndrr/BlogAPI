using BlogAPI.DTOs;

namespace BlogAPI.Services.Interfaces;

public interface IPostService
{ 
    Task<IEnumerable<PostGetDTO>?> GetAllPostsAsync();
    Task<PostGetDTO?> GetPostByIdAsync(long Id);
    Task<IEnumerable<PostGetDTO>?> GetPostsByTagIdAsync(int id);
    Task<PostGetDTO?> CreatePostAsync(PostCreateDTO postCreateDto);
    Task<bool> UpdatePostAsync(long id, PostCreateDTO postCreateDto);
    Task<bool> BanPostAsync(long id);
    Task<bool> DeletePostAsync(long postId, string userID);



}