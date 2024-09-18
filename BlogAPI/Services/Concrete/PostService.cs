using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Mappers;
using BlogAPI.Model;
using BlogAPI.Repositories;
using BlogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly UserManager<AppUser> _userManager;

    public PostService(IPostRepository postRepository, UserManager<AppUser> userManager)
    {
        _postRepository = postRepository;
        _userManager = userManager;
    }

    public async Task<ServiceResult<IEnumerable<PostGetDTO>>> GetAllPostsAsync()
    {
        var posts = await _postRepository.GetAllPostsAsync();
        if (posts == null)
        {
            return ServiceResult<IEnumerable<PostGetDTO>>.FailureResult("Posts not found.");
        }

        var postDtos = posts.Select(post => post.ToDto()).ToList();
        return ServiceResult<IEnumerable<PostGetDTO>>.SuccessResult(postDtos);
    }

    public async Task<ServiceResult<PostGetDTO>> GetPostByIdAsync(long id)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null || post.isBanned || post.IsDeleted)
        {
            return ServiceResult<PostGetDTO>.FailureResult("Post not found or is banned/deleted.");
        }

        return ServiceResult<PostGetDTO>.SuccessResult(post.ToDto());
    }

    public async Task<ServiceResult<IEnumerable<PostGetDTO>>> GetPostsByTagIdAsync(int tagId)
    {
        var posts = await _postRepository.GetPostsByTagIdAsync(tagId);
        if (posts == null || !posts.Any())
        {
            return ServiceResult<IEnumerable<PostGetDTO>>.FailureResult("No posts found for the specified tag.");
        }

        var postDtos = posts.Select(post => post.ToDto()).ToList();
        return ServiceResult<IEnumerable<PostGetDTO>>.SuccessResult(postDtos);
    }

    public async Task<ServiceResult<PostGetDTO>> CreatePostAsync(PostCreateDTO postCreateDto)
    {
        var post = postCreateDto.ToEntity();
        await _postRepository.AddPostAsync(post);

        var success = await CrossTables(postCreateDto, post);
        if (!success)
        {
            return ServiceResult<PostGetDTO>.FailureResult("Error updating cross tables.");
        }

        return ServiceResult<PostGetDTO>.SuccessResult(post.ToDto());
    }

    public async Task<ServiceResult<bool>> UpdatePostAsync(long id, PostCreateDTO postCreateDto)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        if (!string.IsNullOrEmpty(postCreateDto.Title))
        {
            post.Title = postCreateDto.Title;
        }

        if (!string.IsNullOrEmpty(postCreateDto.HtmlContent))
        {
            post.HtmlContent = postCreateDto.HtmlContent;
        }

        post.UpdatedDate = DateTime.Now;
        await _postRepository.UpdatePostAsync(post);

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> BanPostAsync(long id)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        post.isBanned = true;
        await _postRepository.UpdatePostAsync(post);

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> DeletePostAsync(long id, string userId)
    {
        var post = await _postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        var userPost = await _postRepository.GetUserPostAsync(id, userId);
        if (userPost == null)
        {
            return ServiceResult<bool>.FailureResult("User is not authorized to delete this post.");
        }

        post.IsDeleted = true;
        await _postRepository.UpdatePostAsync(post);

        return ServiceResult<bool>.SuccessResult(true);
    }

    private async Task<bool> CrossTables(PostCreateDTO postCreateDto, Post post)
    {
        var userPost = new UserPost
        {
            AuthorId = postCreateDto.UserId,
            PostId = post.Id
        };

        var mainAuthor = await _userManager.FindByIdAsync(postCreateDto.UserId);
        if (mainAuthor == null)
        {
            return false;
        }

        mainAuthor.NumOfPosts++;
        await _userManager.UpdateAsync(mainAuthor);
        await _postRepository.AddUserPostAsync(userPost);

        if (postCreateDto.CoAuhtorsIds != null)
        {
            foreach (var authorId in postCreateDto.CoAuhtorsIds)
            {
                var author = await _userManager.FindByIdAsync(authorId);
                if (author == null)
                {
                    return false;
                }

                author.NumOfPosts++;
                await _userManager.UpdateAsync(author);
                await _postRepository.AddUserPostAsync(new UserPost
                {
                    AuthorId = authorId,
                    PostId = post.Id
                });
            }
        }

        if (postCreateDto.TagIds != null)
        {
            foreach (var tagId in postCreateDto.TagIds)
            {
                var tag = await _postRepository.GetTagPostAsync(tagId);
                if (tag == null)
                {
                    return false;
                }

                await _postRepository.AddTagPostAsync(new TagPost
                {
                    TagId = tagId,
                    PostId = post.Id
                });
            }
        }

        await _postRepository.SaveChangesAsync();
        return true;
    }
}

    
