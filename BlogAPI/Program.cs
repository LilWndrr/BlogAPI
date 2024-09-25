using System.Text;
using BlogAPI.Data;
using BlogAPI.Model;
using BlogAPI.Repositories;
using BlogAPI.Repositories.Concrete;
using BlogAPI.Services;
using BlogAPI.Services.Concrete;
using BlogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            string connection = builder.Configuration.GetConnectionString("Redis");
            options.Configuration = connection;
        } );
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        
        builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(2));
        var key = Encoding.ASCII.GetBytes(builder.Configuration["JWT:Key"]);
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)

                       
                };
            });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        builder.Services.AddAutoMapper(typeof(Program).Assembly);


        builder.Services.AddMemoryCache();
       //Inserting Repositories
       builder.Services.AddScoped<IUserRepository, UserRepository>();
       builder.Services.AddScoped<IPostRepository, PostRepository>();
       builder.Services.AddScoped<IPostLikeRepository, PostLikeRepository>();
       builder.Services.AddScoped<ICommentRepository, CommentRepository>();
       builder.Services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();
       builder.Services.AddScoped<ITagRepository, TagRepository>();
       //Scrutor
       //builder.Services.Decorate<IUserRepository, UserRepository>();
        
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

