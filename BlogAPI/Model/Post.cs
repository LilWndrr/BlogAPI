﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using BlogAPI.Model.Interfaces;

namespace BlogAPI.Model
{
	public class Post:ISoftDeletable
	{

		public long Id { get; set; }

		public string Title { get; set; } = "";
		public string HtmlContent { get; set; } = "";

		public bool IsDeleted { get; set; }
		public bool isBanned { get; set; }
		public DateTime PublicationDate { get; set; } = DateTime.Now;
		public DateTime? UpdatedDate { get; set; }
		public long LikeCount { get; set; }
		public int CommentCount { get; set; }

		[NotMapped]
		public List<String>? CoAuhtorsIds { get; set; }

		[NotMapped]
		public List<string>? TagIds { get; set; }

		public List<PostLike>? PostLikes { get; set; }

		public List<Comment>? Comments { get; set; }
		public List<UserPost>? Authors { get; set; }
		public List<TagPost>? TagPosts { get; set; }





	}
}

