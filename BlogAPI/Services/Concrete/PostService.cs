using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Mappers;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class PostService : IPostService
{
    private readonly ApplicationContext _context;
    private readonly UserManager<AppUser> _userManager;

    public PostService(ApplicationContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ServiceResult<IEnumerable<PostGetDTO>>> GetAllPostsAsync()
    {
        if (_context.Posts == null)
        {
            return ServiceResult<IEnumerable<PostGetDTO>>.FailureResult("Posts table is not available.");
        }

        var posts = await _context.Posts
            .Where(p => !p.isDeleted && !p.isBanned)
            .Include(p => p.PostLikes).ThenInclude(pl => pl.User)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Include(p => p.Authors).ThenInclude(up => up.Author)
            .Include(p => p.TagPosts).ThenInclude(tp => tp.Tag)
            .ToListAsync();

        var postDtos = posts.Select(post => post.ToDto()).ToList();
        return ServiceResult<IEnumerable<PostGetDTO>>.SuccessResult(postDtos);
    }

    public async Task<ServiceResult<PostGetDTO>> GetPostByIdAsync(long id)
    {
        if (_context.Posts == null)
        {
            return ServiceResult<PostGetDTO>.FailureResult("Posts table is not available.");
        }

        var post = await _context.Posts
            .Include(p => p.PostLikes).ThenInclude(pl => pl.User)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Include(p => p.Authors).ThenInclude(up => up.Author)
            .Include(p => p.TagPosts).ThenInclude(tp => tp.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null || post.isBanned || post.isDeleted)
        {
            return ServiceResult<PostGetDTO>.FailureResult("Post not found or is banned/deleted.");
        }

        return ServiceResult<PostGetDTO>.SuccessResult(post.ToDto());
    }

    public async Task<ServiceResult<IEnumerable<PostGetDTO>>> GetPostsByTagIdAsync(int tagId)
    {
        if (_context.Posts == null)
        {
            return ServiceResult<IEnumerable<PostGetDTO>>.FailureResult("Posts table is not available.");
        }

        var posts = await _context.TagPosts
            .Where(tp => tp.TagId == tagId)
            .Select(tp => tp.Post)
            .Where(p => !p.isBanned && !p.isDeleted)
            .ToListAsync();

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
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        var success = await CrossTables(postCreateDto, post);
        if (!success)
        {
            return ServiceResult<PostGetDTO>.FailureResult("Error updating cross tables.");
        }

        return ServiceResult<PostGetDTO>.SuccessResult(post.ToDto());
    }

    public async Task<ServiceResult<bool>> UpdatePostAsync(long id, PostCreateDTO postCreateDto)
    {
        var post = await _context.Posts.FindAsync(id);
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
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> BanPostAsync(long id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        post.isBanned = true;
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> DeletePostAsync(long id, string userId)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        var userPost = await _context.UserPosts
            .FirstOrDefaultAsync(up => up.PostId == id && up.AuthorId == userId);

        if (userPost == null)
        {
            return ServiceResult<bool>.FailureResult("User is not authorized to delete this post.");
        }

        post.isDeleted = true;
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();

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
        _context.UserPosts.Add(userPost);

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
                _context.UserPosts.Add(new UserPost
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
                var tag = await _context.Tags.FindAsync(tagId);
                if (tag == null)
                {
                    return false;
                }

                _context.TagPosts.Add(new TagPost
                {
                    TagId = tagId,
                    PostId = post.Id
                });
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}

    
