using System;
using System.Reflection;
using BlogAPI.Model;
using BlogAPI.Model.Interfaces;
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

            modelBuilder.Entity<PostLike>().HasKey(a => new { UserID = a.UserId, a.PostId });
            modelBuilder.Entity<CommentLike>().HasKey(b => new { b.UserID, b.CommentId });
            modelBuilder.Entity<UserPost>().HasKey(b => new { b.AuthorId, b.PostId });
            modelBuilder.Entity<TagPost>().HasKey(b => new { b.PostId,b.TagId });
            //Usage of global query filter
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {   
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ApplicationContext).GetMethod(nameof(ApplyGlobalFilter), 
                            BindingFlags.NonPublic | BindingFlags.Static)
                        ?.MakeGenericMethod(entityType.ClrType);
                    method?.Invoke(null, new object[] { modelBuilder });
                }
            }

            modelBuilder.Entity<Post>()
           .HasMany(p => p.Comments)
           .WithOne(c => c.Post)
           .HasForeignKey(c => c.PostId)
           .OnDelete(DeleteBehavior.Cascade);

            // Configure the Post-PostLike relationship
            modelBuilder.Entity<Post>()
                .HasMany(p => p.PostLikes)
                .WithOne(pl => pl.Post)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the Comment-CommentLike relationship
            modelBuilder.Entity<Comment>()
                .HasMany(c => c.CommentLikes)
                .WithOne(cl => cl.Comment)
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the Comment-SubComments relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c=>c.ParentComment)
                .WithMany(c => c.SubComments)
                .HasForeignKey(sc => sc.CommentId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        private static void ApplyGlobalFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDeletable
        {
            modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }
        }
    }

    


