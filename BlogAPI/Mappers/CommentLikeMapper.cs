using BlogAPI.DTOs;
using BlogAPI.Model;

namespace BlogAPI.Mappers;

public static class CommentLikeMapper
{
    // Map CommentLike entity to CommentLikeGetDto
    public static CommentLikeGetDto ToDto(this CommentLike commentLike)
    {
        return new CommentLikeGetDto
        {
            UserId = commentLike.UserID,
            CommentId = commentLike.CommentId,
            DateCreated = commentLike.DateCreated
        };
    }

    // Map CommentLikePostDto to CommentLike entity
    public static CommentLike ToEntity(this CommentLikePostDto commentLikePostDto)
    {
        return new CommentLike
        {
            UserID = commentLikePostDto.UserId,
            CommentId = commentLikePostDto.CommentId,
            DateCreated = DateTime.Now // Automatically set the creation date
        };
    }
}
