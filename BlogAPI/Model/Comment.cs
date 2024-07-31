using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogAPI.Model
{
	public class Comment
	{
		public long Id { get; set; }
		[Required]
		public string Content { get; set; } = "";
		public int LikesCount { get; set; }
		public DateTime CreatedDateTime { get; set; } = DateTime.Now;
		public int  CommentCount { get; set; }


		public string? UserID { get; set; }
		public long? CommentId { get; set; }
		public long? PostId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserID))]
		public AppUser? User  { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
		public Post? Post { get; set; }

		[ForeignKey(nameof(CommentId))]
        public Comment? ParentComment { get; set; }




		public List<Comment>? SubComments { get; set; }

       

    }
}

