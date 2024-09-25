using BlogAPI.DTOs;
using BlogAPI.HelperServices;

namespace BlogAPI.Services.Interfaces;

public interface IPostService
{ 
    Task<ServiceResult<IEnumerable<PostGetDTO>>> GetAllPostsAsync();
    Task<ServiceResult<PostGetDTO>> GetPostByIdAsync(long Id);
    Task<ServiceResult<IEnumerable<PostGetDTO>>> GetPostsByTagIdAsync(string id);
    Task<ServiceResult<PostGetDTO>> CreatePostAsync(PostCreateDTO postCreateDto);
    Task<ServiceResult<bool>> UpdatePostAsync(long id, PostCreateDTO postCreateDto);
    Task<ServiceResult<bool>> BanPostAsync(long id);
    Task<ServiceResult<bool>> DeletePostAsync(long postId, string userID);



}