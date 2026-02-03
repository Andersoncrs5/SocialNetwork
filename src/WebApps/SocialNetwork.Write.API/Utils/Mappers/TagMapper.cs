using AutoMapper;
using SocialNetwork.Contracts.DTOs.Tag;
using SocialNetwork.Write.API.dto.Tag;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class TagMapper: Profile
{
    public TagMapper()
    {
        CreateMap<CreateTagDto, TagModel>();
        CreateMap<TagModel, TagDto>().ReverseMap();
    }
}