using AutoMapper;
using SocialNetwork.Contracts.DTOs.PostTag;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.PostTag.Dto;
using SocialNetwork.Write.API.Modules.PostTag.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class PostTagMapper: Profile
{
    public PostTagMapper()
    {
        CreateMap<CreatePostTagDto, PostTagModel>();
        CreateMap<PostTagModel, PostTagDto>().ReverseMap();
    }
}