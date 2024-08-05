using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Model
{
	public class AppUser: IdentityUser
	{

        public string Name { get; set; } = "";
        public string? MiddleName { get; set; }
        public string? FamilyName { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsBanned { get; set; }
        
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public int NumOfPosts { get; set; } 
       
        [NotMapped]
        public string? Password { get; set; }
        [NotMapped]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }


        public List<Comment>? Comments { get; set; }
        public List<PostLike>? PostLikes { get; set; }

        public List<UserPost>? Posts { get; set; }



	}
}

