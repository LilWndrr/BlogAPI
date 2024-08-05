using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Model;
using BlogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services.Concrete;

public class PostService: IPostService
{
    private readonly ApplicationContext _context;
    private readonly UserManager<AppUser> _userManager;

    public PostService(ApplicationContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IEnumerable<PostGetDTO>?> GetAllPostsAsync()
    {
        if (_context.Posts == null)
        {
            return null;
        }
        var posts = await _context.Posts.Where(p=>p.isDeleted==false&&p.isBanned==false).Include(p=>p.PostLikes)
            .Include(p=>p.Comments)!.ThenInclude(c=>c.User)
            .Include(p=>p.Authors).ThenInclude(up=>up.Author)
            .Include(p=>p.TagPosts).ThenInclude(tp=>tp.Tag)
            .Include(p=>p.PostLikes).ThenInclude(pl=>pl.User)
            .ToListAsync();
        var postsDtos = posts.Select(post => new PostGetDTO
            {
                Id = post.Id,
                Title = post.Title,
                HtmlContent = post.HtmlContent,
                PublicationDate = post.PublicationDate,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                Comments = post.Comments?.Select(comment => new CommentGetDto
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        LikesCount = comment.LikesCount,
                        CommentCount = comment.CommentCount,
                        UseriD = comment.UserID
                        
                    }
                ).ToList(),
                Tags = post.TagPosts?.Select(tag=> new TagGetDto
                {
                    ID=tag.Tag.Id,
                    Name = tag.Tag.Name
                }).ToList(),
                Likes = post.PostLikes?.Select(like=> new PostLikeGetDto
                {
                    UserId = like.User.Id
                }).ToList(),
                Authors = post.Authors?.Select(author=> new AppUserGetDto
                {
                    Id = author.AuthorId!,
                    Name = author.Author!.Name,
                    MiddleName = author.Author.MiddleName,
                    FamilyName = author.Author.FamilyName,
                    NumOfPosts = author.Author.NumOfPosts,
                    Gender = author.Author.Gender,
                    BirthDate = author.Author.BirthDate
                }).ToList()
            }
        );
        
        
        
        
        return postsDtos;
    }

    public async Task<PostGetDTO?> GetPostByIdAsync(long id)
    {
        if (_context.Posts == null)
        {
            return null;
        }
        var post = await _context.Posts.Include(p => p.PostLikes)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Include(p => p.Authors).ThenInclude(up => up.Author)
            .Include(p => p.TagPosts).ThenInclude(tp => tp.Tag)
            .Include(p => p.PostLikes).ThenInclude(pl => pl.User).FirstOrDefaultAsync(p => p.Id == id);

        if (post == null||post.isBanned==true||post.isDeleted==true)
        {
            return null;
        }

      
        PostGetDTO? postGetDto = new PostGetDTO
        {
            Id = post.Id,
            Title = post.Title,
            HtmlContent = post.HtmlContent,
            PublicationDate = post.PublicationDate,
            LikeCount = post.LikeCount,
            CommentCount = post.CommentCount,
            Comments = post.Comments?.Select(comment => new CommentGetDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    LikesCount = comment.LikesCount,
                    CommentCount = comment.CommentCount,
                    UseriD = comment.UserID
                        
                }
            ).ToList(),
            Tags = post.TagPosts?.Select(tag=> new TagGetDto
            {
                ID=tag.Tag.Id,
                Name = tag.Tag.Name
            }).ToList(),
            Likes = post.PostLikes?.Select(like=> new PostLikeGetDto
            {
                UserId = like.User.Id
            }).ToList(),
            Authors = post.Authors?.Select(author=> new AppUserGetDto
            {
                Id = author.AuthorId!,
                Name = author.Author!.Name,
                MiddleName = author.Author.MiddleName,
                FamilyName = author.Author.FamilyName,
                NumOfPosts = author.Author.NumOfPosts,
                Gender = author.Author.Gender,
                BirthDate = author.Author.BirthDate
            }).ToList()
        
        };
    
        
        
        return postGetDto;
    }

    public async Task<IEnumerable<PostGetDTO>?> GetPostsByTagIdAsync(int id)
    {
        if (_context.Posts == null)
        {
            return null;
        }
        var posts =  await _context.TagPosts.Where(up => up.TagId == id).Select(up => up.Post).Where(p=>p.isBanned==false&&p.isDeleted==false).ToListAsync();
        if (posts==null)
        {
            return null;
        }
        
        return posts.Select(post => new PostGetDTO
            {
                Id = post.Id,
                Title = post.Title,
                HtmlContent = post.HtmlContent,
                PublicationDate = post.PublicationDate,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                Comments = post.Comments?.Select(comment => new CommentGetDto
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        LikesCount = comment.LikesCount,
                        CommentCount = comment.CommentCount,
                        UseriD = comment.UserID
                        
                    }
                ).ToList(),
                Tags = post.TagPosts?.Select(tag=> new TagGetDto
                {
                    ID=tag.Tag.Id,
                    Name = tag.Tag.Name
                }).ToList(),
                Likes = post.PostLikes?.Select(like=> new PostLikeGetDto
                {
                    UserId = like.User.Id
                }).ToList(),
                Authors = post.Authors?.Select(author=> new AppUserGetDto
                {
                    Id = author.AuthorId!,
                    Name = author.Author!.Name,
                    MiddleName = author.Author.MiddleName,
                    FamilyName = author.Author.FamilyName,
                    NumOfPosts = author.Author.NumOfPosts,
                    Gender = author.Author.Gender,
                    BirthDate = author.Author.BirthDate
                }).ToList()
            }
        );
    }

    public async Task<PostGetDTO?> CreatePostAsync(PostCreateDTO postCreateDto)
    {
        Post post = new Post
        {
            Title = postCreateDto.Title,
            HtmlContent = postCreateDto.Title,
            PublicationDate = DateTime.Now,
            
            

        };
         _context.Posts.Add(post);
         await _context.SaveChangesAsync();
         CrossTables(postCreateDto, post);
         
         return new PostGetDTO
        {
            Id = post.Id,
            Title = post.Title,
            HtmlContent = post.HtmlContent,
            PublicationDate = post.PublicationDate,
            CommentCount = post.CommentCount,
            LikeCount = post.LikeCount
        };;
    }

    public async Task<bool> UpdatePostAsync(long id,PostCreateDTO postCreateDto )
    {
        if (_context.Posts != null)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return false;
            }
            if (postCreateDto.Title!="")
            {
                post.Title = postCreateDto.Title;
            }

            if (postCreateDto.HtmlContent!="")
            {
                post.HtmlContent = postCreateDto.HtmlContent;
            }
        
            post.UpdatedDate = DateTime.Now;
            _context.Posts.Update(post);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BanPostAsync(long id)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            return false;
        }

        post.isBanned = true;
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeletePostAsync(long id,string userId)
    {
        if (_context.Posts == null)
        {
            return false;
        }
        var post = _context.Posts.FirstOrDefault(p => p.Id == id);
        UserPost userPost = _context.UserPosts.FirstOrDefault(up => up.PostId == post.Id && up.AuthorId == userId);
        if (userPost == null)
        {
            return false;
        }
        if (post == null)
        {
            return false;
        }

        post.isDeleted = true;
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
        return true;
    }
    
    private async Task<bool> CrossTables(PostCreateDTO postCreateDto, Post post)
    {
        
        UserPost userPost1 = new UserPost();
        userPost1.AuthorId = postCreateDto.UserId;
        userPost1.PostId = post.Id;
        AppUser mainAuthor = await _userManager.FindByIdAsync(postCreateDto.UserId);
        mainAuthor.NumOfPosts++;
        await _userManager.UpdateAsync(mainAuthor);
        _context.UserPosts.Add(userPost1);

        if (postCreateDto.CoAuhtorsIds  != null)
        {
            foreach (var authorId in postCreateDto.CoAuhtorsIds)
            {
                AppUser author = await _userManager.FindByIdAsync(authorId);
                
                if (author == null)
                {
                    return false;
                }

                author.NumOfPosts++;
                await _userManager.UpdateAsync(author);
                UserPost userPost = new UserPost();
                userPost.AuthorId = authorId;
                userPost.PostId = post.Id;
                _context.UserPosts.Add(userPost);
            }
        }
        if (postCreateDto.TagIds!=null)
        {
            foreach (var tagId in postCreateDto.TagIds)
            {
                Tag tag =  _context.Tags.FirstOrDefault(t=> t.Id==tagId);
                if (tag == null)
                {
                    return false;
                }
                TagPost tagPost = new TagPost();
                tagPost.TagId = tagId;
                tagPost.PostId = post.Id;
                _context.TagPosts.Add(tagPost);

            }
        }

        await _context.SaveChangesAsync();

        return true;
    } 
    
    
}