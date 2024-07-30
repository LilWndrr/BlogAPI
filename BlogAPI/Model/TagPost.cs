using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Model
{
	public class TagPost
	{
		public long  PostId { get; set; }
		public int TagId { get; set; }

		[ForeignKey(nameof(PostId))]
		public Post? Post { get; set; }

		[ForeignKey(nameof(TagId))]
		public Tag? Tag { get; set; }
	}
}

