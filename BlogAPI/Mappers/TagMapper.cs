﻿using BlogAPI.DTOs;
using BlogAPI.Model;

namespace BlogAPI.Mappers;

public static class TagMapper
{
    // Map Tag entity to TagGetDto
    public static TagGetDto ToDto(this Tag tag)
    {
        return new TagGetDto
        {
            ID = tag.Id,
            Name = tag.Name
        };
    }

    // Map TagGetDto to Tag entity (useful for creating or updating tags)
    public static Tag ToEntity(this TagGetDto tagGetDto)
    {
        return new Tag
        {
            Id = tagGetDto.ID,
            Name = tagGetDto.Name
        };
    }
}
