using AutoMapper;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Write.API.Models;
using SocialNetwork.Write.API.Modules.Post.Dto;
using SocialNetwork.Write.API.Modules.Post.Model;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class PostMapper: Profile
{
    public PostMapper()
    {
        CreateMap<CreatePostDto, PostModel>();
        CreateMap<PostModel, PostDto>().ReverseMap();
    }
}