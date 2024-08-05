using BlogAPI.Data;
using BlogAPI.Model;
using BlogAPI.Services;
using BlogAPI.Services.Concrete;
using BlogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext")));
        builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
        builder.Services.AddControllers().AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        var emailConfig = builder.Configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();
        builder.Services.AddSingleton(emailConfig);
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        //Configuring email-sender
      
        
       
           
        
        //Inserting Application Services
        builder.Services.AddScoped<IEmailSender, EmailSender>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<ICommentLikeService, CommentLikeService>();
        builder.Services.AddScoped<IPostLikeService, PostLikeService>();
        
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        

        app.MapControllers();

        app.Run();
    }
}

