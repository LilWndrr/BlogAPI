using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BlogAPI.Model;

namespace BlogAPI.DTOs;

public class PostGetDTO
{
    public long Id { get; set; }

    public string? Title { get; set; } = "";
    public string? HtmlContent { get; set; } = "";

    public DateTime PublicationDate { get; set; }

    public long LikeCount { get; set; }
    public int CommentCount { get; set; }
    public List<TagGetDto>? Tags { get; set; }

    public List<PostLikeGetDto>? Likes { get; set; }
    public List<CommentGetDto>? Comments { get; set; }
    public List<AppUserGetDto>? Authors { get; set; }
}

public class PostCreateDTO
{
   

    public string Title { get; set; } = "";
    public string HtmlContent { get; set; } = "";
    [JsonIgnore]
    public string  UserId { get; set; }="";
    public DateTime PublicationDate { get; set; } = DateTime.Now;
    
    public List<String>? CoAuhtorsIds { get; set; } 
    
    public List<string>? TagIds { get; set; }
    
}

