namespace BlogAPI.DTOs;

public class CommentGetDto
{
    public long Id { get; set; }
   
    public string Content { get; set; } = "";
    public int LikesCount { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public int  CommentCount { get; set; }
    public string? UseriD { get; set; } = "";
}

public class CommentCreateDto
{
   
   
    public string Content { get; set; } = "";
    public string? UserId { get; set; }
    public long? CommentId { get; set; }
    public long? PostId { get; set; }
    
}