using BlogAPI.DTOs;
using BlogAPI.HelperServices;
using BlogAPI.Model;
using BlogAPI.Repositories;
using BlogAPI.Services.Interfaces;

namespace BlogAPI.Services.Concrete;

public class PostLikeService : IPostLikeService
{
    private readonly IPostLikeRepository _postLikeRepository;
    private readonly IPostRepository _postRepository;

    public PostLikeService(IPostLikeRepository postLikeRepository,IPostRepository postRepository)
    {
        _postLikeRepository = postLikeRepository;
        _postRepository = postRepository;
    }

    public async Task<ServiceResult<IEnumerable<PostLikeGetDto>>> GetPostLikesByPostIdAsync(long postId)
    {
        var postLikes = await _postLikeRepository.GetPostLikesByPostIdAsync(postId);
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
        var postLikes = await _postLikeRepository.GetUserPostLikesAsync(userId);
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
        var post = await _postRepository.GetPostByIdAsync(postLikeCreateDto.PostId);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        var postLike = new PostLike
        {
            UserId = postLikeCreateDto.UserId,
            PostId = postLikeCreateDto.PostId,
            CreatedDate = DateTime.Now
        };

        post.LikeCount++;
        await _postRepository.UpdatePostAsync(post);
        await _postLikeRepository.AddPostLikeAsync(postLike);

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> DeletePostLikeAsync(long postId, string userId)
    {
        var postLike = await _postLikeRepository.FindPostLikeAsync(postId, userId);
        if (postLike == null)
        {
            return ServiceResult<bool>.FailureResult("Post like not found.");
        }

        var post = await _postRepository.GetPostByIdAsync(postId);
        if (post == null)
        {
            return ServiceResult<bool>.FailureResult("Post not found.");
        }

        post.LikeCount--;
        await _postRepository.UpdatePostAsync(post);
        await _postLikeRepository.RemovePostLikeAsync(postLike);

        return ServiceResult<bool>.SuccessResult(true);
    }
}
