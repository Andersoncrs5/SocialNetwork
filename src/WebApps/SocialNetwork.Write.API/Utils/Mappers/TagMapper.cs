using AutoMapper;
using SocialNetwork.Contracts.DTOs.Tag;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Tag.Dto;
using SocialNetwork.Write.API.Modules.Tag.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class TagMapper: Profile
{
    public TagMapper()
    {
        CreateMap<CreateTagDto, TagModel>();
        CreateMap<TagModel, TagDto>().ReverseMap();
    }
}