using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlogAPI.Model
{
	public class Tag
	{
		
		[Key]
		public string Name { get; set; } = "";
		[JsonIgnore]
        public List<TagPost>? tagPosts { get; set; }
	}
}

