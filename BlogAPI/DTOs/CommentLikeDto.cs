namespace BlogAPI.DTOs;

public class CommentLikeGetDto
{
    public string? UserId { get; set; } 
    public long? CommentId  { get; set; }
    public DateTime DateCreated { get; set; }
   
}
public class CommentLikePostDto
{
    public string UserId { get; set; } = "";
    public long CommentId  { get; set; }
    
}