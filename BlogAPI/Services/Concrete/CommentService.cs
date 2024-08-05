using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class CommentService:ICommentService

{
    private readonly ApplicationContext _context;
    public CommentService(ApplicationContext context)
    {
        _context = context;
    }
    
    public async Task<CommentGetDto?> GetCommentAsync(long id)
    {
        if (_context.Comments == null)
        {
            return null;
        }

        var comment = await _context.Comments.FindAsync(id);
        if (comment != null)
        {
            return new CommentGetDto
            {
                Id = comment.Id,
                Content = comment.Content,
                LikesCount = comment.LikesCount,
                CommentCount = comment.CommentCount,
                UseriD = comment.UserID
            };
        }

        return null;
    }

    public async Task<CommentGetDto?> PutCommentAsync(long id, CommentCreateDto commentCreateDto)
    {
        if (_context.Comments!=null)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.Content = commentCreateDto.Content;
                comment.UpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
            } 
            return new CommentGetDto
            {
                Id = comment!.Id,
                Content = comment.Content,
                LikesCount = comment.LikesCount,
                CommentCount = comment.CommentCount,
                UseriD = comment.UserID
            };
        }

        return null;
       
    }

    public async Task<IEnumerable<CommentGetDto>?> GetCommentByPostIdAsync(long postId)
    {
        if (_context.Comments != null)
        {
            var comments =await _context.Comments.Where(c => c.PostId == postId && c.CommentId == null).ToListAsync();
            if (comments != null)
            {
                return comments.Select(comment => new CommentGetDto
                {
                    Id = comment!.Id,
                    Content = comment.Content,
                    LikesCount = comment.LikesCount,
                    CommentCount = comment.CommentCount,
                    UseriD = comment.UserID
                });
            }
            
        }
        return null;
      
    }

    public async Task<IEnumerable<CommentGetDto>?> GetCommentByParentIdAsync(long parentId)
    {
        if (_context.Comments != null)
        {
            var comments = await _context.Comments.Where(c => c.CommentId == parentId).ToListAsync();
            if (comments != null)
            {
                return comments.Select(comment => new CommentGetDto
                {
                    Id = comment!.Id,
                    Content = comment.Content,
                    LikesCount = comment.LikesCount,
                    CommentCount = comment.CommentCount,
                    UseriD = comment.UserID
                });
            }
        }

        return null;
    }

    public async Task<CommentGetDto?> PostCommentAsync(CommentCreateDto commentCreateDto)
    {
        if (_context.Comments != null)
        {
            Comment comment = new Comment
            {
                Content = commentCreateDto.Content,
                CommentId = commentCreateDto.CommentId,
                
                UserID = commentCreateDto.UserId
            };
            if (commentCreateDto.CommentId == null)
            {
                comment.CommentId = commentCreateDto.CommentId;
            }
            if (commentCreateDto.CommentId != null)
            {
                var parentComment= _context.Comments.FirstOrDefault(c => c.Id == comment.CommentId);
                comment.PostId = parentComment!.PostId;
                
                parentComment.CommentCount++;
                _context.Comments.Update(parentComment);

            }
            var post=_context.Posts!.FirstOrDefault(p => p.Id == comment.PostId);
            post!.CommentCount++;
            _context.Posts!.Update(post);
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
          
             return new CommentGetDto
            {
                
                Content = comment.Content,
                LikesCount = comment.LikesCount,
                CommentCount = comment.CommentCount,
                UseriD = comment.UserID
            };
        }

        return null;
    }

    public async Task<bool> DeleteCommentAsync(long id)
    {
        if (_context.Comments == null) return false;
        
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return false;
            
        comment.IsDeleted = true;
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        return true;
            
        
        
    }
}