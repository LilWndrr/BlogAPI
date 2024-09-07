using BlogAPI.DTOs;
using BlogAPI.Model;

namespace BlogAPI.Mappers;

public static class PostMapper
{
    // Map Post entity to PostGetDTO
    public static PostGetDTO ToDto(this Post post)
    {
        return new PostGetDTO
        {
            Id = post.Id,
            Title = post.Title,
            HtmlContent = post.HtmlContent,
            PublicationDate = post.PublicationDate,
            LikeCount = post.LikeCount,
            CommentCount = post.CommentCount,
            Tags = post.TagPosts?.Select(tp => tp.Tag.ToDto()).ToList(),
            Likes = post.PostLikes?.Select(pl => pl.ToDto()).ToList(),
            Comments = post.Comments?.Select(c => c.ToDto()).ToList(),
            Authors = post.Authors?.Select(a => a.Author.ToDto()).ToList()
        };
    }

    // Map PostCreateDTO to Post entity
    public static Post ToEntity(this PostCreateDTO postCreateDto)
    {
        return new Post
        {
            Title = postCreateDto.Title,
            HtmlContent = postCreateDto.HtmlContent,
            PublicationDate = postCreateDto.PublicationDate,
            CoAuhtorsIds = postCreateDto.CoAuhtorsIds,
            TagIds = postCreateDto.TagIds,
            LikeCount = 0, // Default like count for new posts
            CommentCount = 0, // Default comment count for new posts
            
        };
    }

    // Update an existing Post entity with PostCreateDTO values
    public static void UpdateEntity(this Post post, PostCreateDTO postCreateDto)
    {
        post.Title = postCreateDto.Title;
        post.HtmlContent = postCreateDto.HtmlContent;
        post.PublicationDate = postCreateDto.PublicationDate;
        post.CoAuhtorsIds = postCreateDto.CoAuhtorsIds;
        post.TagIds = postCreateDto.TagIds;
        post.UpdatedDate = DateTime.Now; // Set updated date when post is modified
    }
}
