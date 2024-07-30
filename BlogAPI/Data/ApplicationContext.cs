using System;
using BlogAPI.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Data
{
	public class ApplicationContext: IdentityDbContext<AppUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<PostLike>? PostLikes { get; set; }
        public DbSet<Post>? Posts { get; set; }
        public DbSet<Comment>? Comments { get; set; }
        public DbSet<UserPost>? UserPosts { get; set; }
        public DbSet<CommentLike>? CommentLikes { get; set; }
        public DbSet<Tag>? Tags { get; set; }
        public DbSet<TagPost>? TagPosts { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<PostLike>().HasKey(a => new { a.UserID, a.PostId });
            modelBuilder.Entity<CommentLike>().HasKey(b => new { b.UserID, b.CommentId });
            modelBuilder.Entity<UserPost>().HasKey(b => new { b.AuthorId, b.PostId });
            modelBuilder.Entity<TagPost>().HasKey(b => new { b.TagId,b.PostId });




            base.OnModelCreating(modelBuilder);
        }
    }

    
}

