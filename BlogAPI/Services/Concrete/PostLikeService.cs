using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Mappers;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class PostLikeService : IPostLikeService
{
    private readonly ApplicationContext _context;

    public PostLikeService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<IEnumerable<PostLikeGetDto>>> GetPostLikesByPostIdAsync(long postId)
    {
        if (_context.PostLikes == null)
        {
            return ServiceResult<IEnumerable<PostLikeGetDto>>.FailureResult("PostLikes table is not available.");
        }

        var postLikes = await _context.PostLikes.Where(pl => pl.PostId == postId).ToListAsync();
        if (postLikes == null || !postLikes.Any())
        {
            return ServiceResult<IEnumerable<PostLikeGetDto>>.FailureResult("No likes found for the specified post.");
        }

        var postLikeDtos = postLikes.Select(pl => new PostLikeGetDto
        {
            UserId = pl.UserId,
            DateCreated = pl.CreatedDate
        });

        return ServiceResult<IEnumerable<PostLikeGetDto>>.SuccessResult(postLikeDtos);
    }

    public async Task<ServiceResult<IEnumerable<PostLikeGetDto>>> GetUserPostLikesAsync(string userId)
    {
        if (_context.PostLikes == null)
        {
            return ServiceResult<IEnumerable<PostLikeGetDto>>.FailureResult("PostLikes table is not available.");
        }

        var postLikes = await _context.PostLikes.Where(pl => pl.UserId == userId).ToListAsync();
        if (postLikes == null || !postLikes.Any())
        {
            return ServiceResult<IEnumerable<PostLikeGetDto>>.FailureResult("No likes found for the specified user.");
        }

        var postLikeDtos = postLikes.Select(pl => new PostLikeGetDto
        {
            UserId = pl.UserId,
            DateCreated = pl.CreatedDate
        });

        return ServiceResult<IEnumerable<PostLikeGetDto>>.SuccessResult(postLikeDtos);
    }

    public async Task<ServiceResult<bool>> CreatePostLikeAsync(PostLikeCreateDto postLikeCreateDto)
    {
        if (_context.PostLikes == null || _context.Posts == null)
        {
            return ServiceResult<bool>.FailureResult("Required tables are not available.");
        }

        var postLike = new PostLike
        {
            UserId = postLikeCreateDto.UserId,
            PostId = postLikeCreateDto.PostId,
            CreatedDate = DateTime.Now
        };

        var post = await _context.Posts.FindAsync(postLike.PostId);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        post.LikeCount++;
        _context.Posts.Update(post);
        _context.PostLikes.Add(postLike);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> DeletePostLikeAsync(long postId, string userId)
    {
        if (_context.PostLikes == null || _context.Posts == null)
        {
            return ServiceResult<bool>.FailureResult("Required tables are not available.");
        }

        var postLike = await _context.PostLikes.FindAsync(postId, userId);
        if (postLike == null)
        {
            return ServiceResult<bool>.FailureResult("Post like not found.");
        }

        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        post.LikeCount--;
        _context.Posts.Update(post);
        _context.PostLikes.Remove(postLike);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.SuccessResult(true);
    }
}
