using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Model
{
	public class UserPost
	{
		public string? AuthorId { get; set; } = "";
		public long? PostId { get; set; }

		[ForeignKey(nameof(AuthorId))]
		public AppUser? Author { get; set; }
		[ForeignKey(nameof(PostId))]
		public Post? Post { get; set; }


	}
}

