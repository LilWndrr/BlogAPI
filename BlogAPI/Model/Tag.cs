using System;
using System.Text.Json.Serialization;

namespace BlogAPI.Model
{
	public class Tag
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		[JsonIgnore]
        public List<TagPost>? tagPosts { get; set; }
	}
}

