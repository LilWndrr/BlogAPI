using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Model
{
	public class PostLike
	{
		public string? UserID { get; set; }
		public long? PostId { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.Now;

		[ForeignKey(nameof(UserID))]
		public AppUser? User { get; set; }

		[ForeignKey(nameof(PostId))]
		public Post? Post { get; set; }


	}
}

