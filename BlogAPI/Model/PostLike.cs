using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Model
{
	public class PostLike
	{
		public string? UserId { get; set; }
		public long? PostId { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.Now;

		[ForeignKey(nameof(UserId))]
		public AppUser? User { get; set; }

		[ForeignKey(nameof(PostId))]
		public Post? Post { get; set; }


	}
}

