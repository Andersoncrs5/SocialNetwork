using AutoMapper;
using SocialNetwork.Write.API.dto.PostCategory;
using SocialNetwork.Write.API.Models;

namespace SocialNetwork.Write.API.Utils.Mappers;

public class PostCategoryMapper: Profile
{
    public PostCategoryMapper()
    {
        CreateMap<PostCategoryModel, CreatePostCategoryDto>().ReverseMap();
        CreateMap<CreatePostCategoryDto, PostCategoryModel>();
    }
}