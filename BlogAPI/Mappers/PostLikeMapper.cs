using BlogAPI.DTOs;
using BlogAPI.Model;

namespace BlogAPI.Mappers;

public static class PostLikeMapper
{
    // Map PostLike entity to PostLikeGetDto
    public static PostLikeGetDto ToDto(this PostLike postLike)
    {
        return new PostLikeGetDto
        {
            UserId = postLike.UserId,
            PostId = postLike.PostId,
            DateCreated = postLike.CreatedDate
        };
    }

    // Map PostLikeCreateDto to PostLike entity
    public static PostLike ToEntity(this PostLikeCreateDto postLikeCreateDto)
    {
        return new PostLike
        {
            UserId = postLikeCreateDto.UserId,
            PostId = postLikeCreateDto.PostId,
            CreatedDate = DateTime.Now // Automatically set creation date
        };
    }
}
