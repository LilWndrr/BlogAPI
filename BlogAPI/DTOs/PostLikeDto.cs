namespace BlogAPI.DTOs;

public class PostLikeGetDto
{
    public string? UserId { get; set; } 
    public long? PostId  { get; set; }
    public DateTime DateCreated { get; set; }
    
}

public class PostLikeCreateDto
{
    public string? UserId { get; set; } 
    public long? PostId  { get; set; }
 
    
}