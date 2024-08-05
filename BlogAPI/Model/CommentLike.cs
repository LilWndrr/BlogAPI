using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Model
{
	public class CommentLike
	{
		public string UserID { get; set; } = "";
		public long CommentId  { get; set; }
		public DateTime DateCreated { get; set; }
	
		[ForeignKey(nameof(UserID))]
		public AppUser? User { get; set; }
		[ForeignKey(nameof(CommentId))]
		public Comment? Comment { get; set; }

	}
}

