using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class PostLikeService:IPostLikeService
{
    private readonly ApplicationContext _context;

    public PostLikeService(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<PostLikeGetDto>?> GetPostLikesByPostIdAsync(long postId)
    {
        if (_context.PostLikes != null)
        {
            var postLikes = await _context.PostLikes.Where(pl => pl.PostId == postId).ToListAsync();
            if (postLikes != null)
            {
                return postLikes.Select(postLike => new PostLikeGetDto
                {
                    UserId = postLike.UserId,
                    DateCreated = postLike.CreatedDate
                });
            }
        }

        return null;
    }

    public async Task<IEnumerable<PostLikeGetDto>?> GetUserPostLikesAsync(string id)
    {
        if (_context.PostLikes != null)
        {
            var postLikes = await _context.PostLikes.Where(pl => pl.UserId == id).ToListAsync();
            if (postLikes != null)
            {
                return postLikes.Select(postLike => new PostLikeGetDto
                {
                   PostId = postLike.PostId,
                   DateCreated = postLike.CreatedDate
                });
            }
        }

        return null;
    }

    public async Task<bool> CreatePostLikeAsync(PostLikeCreateDto postLikeCreateDto)
    {
        if (_context.PostLikes != null)
        {
            PostLike postLike = new PostLike
            {
                UserId = postLikeCreateDto.UserId,
                PostId = postLikeCreateDto.PostId,
                CreatedDate = DateTime.Now
            };
            var post =  _context.Posts?.FirstOrDefault(p => p.Id == postLike.PostId);
            if (post != null)
            {
                post.LikeCount++;
                _context.Posts!.Update(post);
                _context.PostLikes.Add(postLike);
                await _context.SaveChangesAsync();
            }
        }

        return false;
    }

    public async Task<bool> DeletePostLikeAsync(long postId, string userId)
    {
        if (_context.PostLikes != null)
        {
            var postLike = await _context.PostLikes.FindAsync(postId, userId);
            if (postLike != null)
            {
                if (_context.Posts != null)
                {
                    var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
                    if (post != null)
                    {
                        post.LikeCount--;
                        _context.Posts.Update(post);
                    }
                }
                _context.PostLikes.Remove(postLike);
                await _context.SaveChangesAsync();
                return true;
            }

            
        }

        return false;
    }
}