using BlogAPI.DTOs;
using BlogAPI.Model;

namespace BlogAPI.Mappers;

public static class CommentMapper
{
    // Map Comment entity to CommentGetDto
    public static CommentGetDto ToDto(this Comment comment)
    {
        return new CommentGetDto
        {
            Id = comment.Id,
            Content = comment.Content,
            LikesCount = comment.LikesCount,
            CreatedDateTime = comment.CreatedDateTime,
            CommentCount = comment.CommentCount,
            UseriD = comment.UserID
        };
    }

    // Map CommentCreateDto to Comment entity
    public static Comment ToEntity(this CommentCreateDto commentCreateDto)
    {
        return new Comment
        {
            Content = commentCreateDto.Content,
            UserID = commentCreateDto.UserId,
            CommentId = commentCreateDto.CommentId,
            PostId = commentCreateDto.PostId,
            CreatedDateTime = DateTime.Now,
            LikesCount = 0, // Default to 0 since it's a new comment
            IsDeleted = false,
            CommentCount = 0 // Default to 0 for new comments
        };
    }

    // Update existing Comment entity with CommentCreateDto values (for edits)
    public static void UpdateEntity(this Comment comment, CommentCreateDto commentCreateDto)
    {
        comment.Content = commentCreateDto.Content;
        comment.UserID = commentCreateDto.UserId;
        comment.CommentId = commentCreateDto.CommentId;
        comment.PostId = commentCreateDto.PostId;
        comment.UpdateDateTime = DateTime.Now; // Set updated time
    }
}
