using AutoMapper;
using SocialNetwork.Contracts.DTOs.Post;
using SocialNetwork.Write.API.dto.Posts;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class PostMapper: Profile
{
    public PostMapper()
    {
        CreateMap<CreatePostDto, PostModel>();
        CreateMap<PostModel, PostDto>().ReverseMap();
    }
}